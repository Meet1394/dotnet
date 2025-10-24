using PersonalCloudDrive.Models;

namespace PersonalCloudDrive.Services
{
    public class FileService
    {
        private readonly SupabaseService _supabase;
        private readonly IConfiguration _configuration;

        public FileService(SupabaseService supabase, IConfiguration configuration)
        {
            _supabase = supabase;
            _configuration = configuration;
        }

        // Get user's files
        public async Task<List<FileModel>> GetUserFilesAsync(string userId, int? folderId = null)
        {
            var client = _supabase.GetClient();
            
            // Build query step by step to avoid parsing errors
            var query = client.From<FileModel>();
            
            // Apply filters one at a time
            var allFiles = await query.Get();
            
            // Filter in memory to avoid Supabase parsing issues
            var filtered = allFiles.Models
                .Where(f => f.UserId == userId && f.IsDeleted == false)
                .ToList();
            
            if (folderId.HasValue)
            {
                filtered = filtered.Where(f => f.FolderId == folderId.Value).ToList();
            }
            else
            {
                filtered = filtered.Where(f => f.FolderId == null).ToList();
            }
            
            return filtered;
        }

        // Get user's folders
        public async Task<List<FolderModel>> GetUserFoldersAsync(string userId, int? parentFolderId = null)
        {
            var client = _supabase.GetClient();
            
            var query = client.From<FolderModel>();
            var allFolders = await query.Get();
            
            // Filter in memory
            var filtered = allFolders.Models
                .Where(f => f.UserId == userId && f.IsDeleted == false)
                .ToList();
            
            if (parentFolderId.HasValue)
            {
                filtered = filtered.Where(f => f.ParentFolderId == parentFolderId.Value).ToList();
            }
            else
            {
                filtered = filtered.Where(f => f.ParentFolderId == null).ToList();
            }
            
            return filtered;
        }

        // Create folder
        public async Task<FolderModel> CreateFolderAsync(string userId, string folderName, int? parentFolderId = null)
        {
            var client = _supabase.GetClient();
            var folder = new FolderModel
            {
                UserId = userId,
                FolderName = folderName,
                ParentFolderId = parentFolderId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            var response = await client.From<FolderModel>().Insert(folder);
            return response.Models.First();
        }

        // Upload file
        public async Task<FileModel> UploadFileAsync(string userId, IFormFile file, int? folderId = null)
        {
            // Check storage limit
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var fileSizeMB = (decimal)file.Length / 1024 / 1024;
            if (user.StorageUsedMB + fileSizeMB > user.StorageLimitMB)
                throw new Exception("Storage limit exceeded");

            // Upload to Supabase Storage
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var storageUrl = await _supabase.UploadFileAsync(userId, file.FileName, fileBytes);
            
            // Extract relative path
            var bucketName = _supabase.GetBucketName();
            var filePath = $"{userId}/{Guid.NewGuid()}_{file.FileName}";

            // Save file metadata to database
            var client = _supabase.GetClient();
            var fileModel = new FileModel
            {
                UserId = userId,
                FileName = file.FileName,
                FilePath = filePath,
                FileType = Path.GetExtension(file.FileName),
                FileSize = file.Length,
                FolderId = folderId,
                StorageUrl = storageUrl,
                UploadedOn = DateTime.UtcNow,
                IsDeleted = false,
                Version = 1
            };

            var response = await client.From<FileModel>().Insert(fileModel);

            // Update user storage
            user.StorageUsedMB += fileSizeMB;
            await client.From<User>()
                .Where(u => u.Id == userId)
                .Set(u => u.StorageUsedMB, user.StorageUsedMB)
                .Update();

            return response.Models.First();
        }

        // Delete file (soft delete)
        public async Task DeleteFileAsync(int fileId, string userId)
        {
            var client = _supabase.GetClient();
            
            // Get all files first, then filter in memory
            var allFiles = await client.From<FileModel>().Get();
            var file = allFiles.Models.FirstOrDefault(f => 
                f.Id == fileId && 
                f.UserId == userId && 
                f.IsDeleted == false);

            if (file == null)
                throw new Exception("File not found");

            // Soft delete
            await client.From<FileModel>()
                .Where(f => f.Id == fileId)
                .Set(f => f.IsDeleted, true)
                .Update();

            // Delete from storage
            try
            {
                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    await _supabase.DeleteFileAsync(file.FilePath, userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to delete from storage: {ex.Message}");
            }

            // Update user storage
            var user = await GetUserAsync(userId);
            if (user != null)
            {
                user.StorageUsedMB -= (decimal)file.FileSize / 1024 / 1024;
                if (user.StorageUsedMB < 0) user.StorageUsedMB = 0;
                
                await client.From<User>()
                    .Where(u => u.Id == userId)
                    .Set(u => u.StorageUsedMB, user.StorageUsedMB)
                    .Update();
            }
        }

        // Delete folder
        public async Task DeleteFolderAsync(int folderId, string userId)
        {
            var client = _supabase.GetClient();

            // Get all folders, filter in memory
            var allFolders = await client.From<FolderModel>().Get();
            var folder = allFolders.Models.FirstOrDefault(f => 
                f.Id == folderId && 
                f.UserId == userId && 
                f.IsDeleted == false);

            if (folder == null)
                throw new Exception("Folder not found");

            // Get all files in folder
            var files = await GetAllFilesInFolderAsync(folderId, userId);

            // Soft delete folder
            await client.From<FolderModel>()
                .Where(f => f.Id == folderId)
                .Set(f => f.IsDeleted, true)
                .Update();

            // Delete all files
            foreach (var file in files)
            {
                try
                {
                    await DeleteFileAsync(file.Id, userId);
                }
                catch
                {
                    // Continue even if one fails
                }
            }
        }

        // Get all files in folder recursively
        public async Task<List<FileModel>> GetAllFilesInFolderAsync(int folderId, string userId)
        {
            var files = new List<FileModel>();
            
            // Get files in current folder
            var currentFiles = await GetUserFilesAsync(userId, folderId);
            files.AddRange(currentFiles);

            // Get subfolders
            var subfolders = await GetUserFoldersAsync(userId, folderId);

            // Recursively get files from subfolders
            foreach (var subfolder in subfolders)
            {
                var subfolderFiles = await GetAllFilesInFolderAsync(subfolder.Id, userId);
                files.AddRange(subfolderFiles);
            }

            return files;
        }

        // Get folder
        public async Task<FolderModel?> GetFolderAsync(int folderId, string userId)
        {
            var client = _supabase.GetClient();
            var allFolders = await client.From<FolderModel>().Get();
            
            return allFolders.Models.FirstOrDefault(f => 
                f.Id == folderId && 
                f.UserId == userId && 
                f.IsDeleted == false);
        }

        // Get user by ID
        private async Task<User?> GetUserAsync(string userId)
        {
            var client = _supabase.GetClient();
            var allUsers = await client.From<User>().Get();
            return allUsers.Models.FirstOrDefault(u => u.Id == userId);
        }

        // Search files
        public async Task<List<FileModel>> SearchFilesAsync(string userId, string searchTerm)
        {
            var files = await GetUserFilesAsync(userId, null);
            return files
                .Where(f => f.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Get folder path
        public async Task<string> GetFolderPathAsync(int? folderId)
        {
            if (!folderId.HasValue)
                return "/";

            var path = new List<string>();
            var currentFolderId = folderId;
            var client = _supabase.GetClient();

            while (currentFolderId.HasValue)
            {
                var allFolders = await client.From<FolderModel>().Get();
                var folder = allFolders.Models.FirstOrDefault(f => f.Id == currentFolderId.Value);

                if (folder == null)
                    break;

                path.Add(folder.FolderName);
                currentFolderId = folder.ParentFolderId;
            }

            path.Reverse();
            return "/" + string.Join("/", path);
        }

        // Get dashboard data
        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var user = await GetUserAsync(userId);
            var files = await GetUserFilesAsync(userId, null);
            var folders = await GetUserFoldersAsync(userId, null);

            var recentFiles = files.OrderByDescending(f => f.UploadedOn)
                .Take(10)
                .ToList();

            return new DashboardViewModel
            {
                StorageUsed = user?.StorageUsedMB ?? 0,
                StorageLimit = user?.StorageLimitMB ?? 1024,
                TotalFiles = files.Count,
                TotalFolders = folders.Count,
                RecentFiles = recentFiles,
                UserName = user?.FullName ?? "User"
            };
        }
    }
}