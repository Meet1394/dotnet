using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalCloudDrive.Services;
using PersonalCloudDrive.Models;
using PersonalCloudDrive.Controllers.Models;
using System.Security.Claims;

namespace PersonalCloudDrive.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly FileService _fileService;
        private readonly SupabaseService _supabase;

        public FileController(FileService fileService, SupabaseService supabase)
        {
            _fileService = fileService;
            _supabase = supabase;
        }

        // File Manager View
        public async Task<IActionResult> Manager(int? folderId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            try
            {
                var files = await _fileService.GetUserFilesAsync(userId, folderId);
                var folders = await _fileService.GetUserFoldersAsync(userId, folderId);
                var path = await _fileService.GetFolderPathAsync(folderId);
                var dashboard = await _fileService.GetDashboardDataAsync(userId);

                var model = new FileManagerViewModel
                {
                    UserName = dashboard.UserName ?? "User",
                    Files = files ?? new List<FileModel>(),
                    Folders = folders ?? new List<FolderModel>(),
                    CurrentFolderId = folderId,
                    CurrentFolderPath = path ?? "/",
                    StorageUsed = (double)dashboard.StorageUsed,
                    StorageLimit = (double)dashboard.StorageLimit,
                    CurrentFilterType = null
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading files: {ex.Message}";

                var emptyModel = new FileManagerViewModel
                {
                    UserName = "User",
                    Files = new List<FileModel>(),
                    Folders = new List<FolderModel>(),
                    CurrentFolderId = folderId,
                    CurrentFolderPath = "/",
                    StorageUsed = 0,
                    StorageLimit = 1024,
                    CurrentFilterType = null
                };

                return View(emptyModel);
            }
        }

        // Upload File
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int? folderId = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file selected" });

                await _fileService.UploadFileAsync(userId, file, folderId);

                return Json(new { success = true, message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Create Folder
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                if (string.IsNullOrEmpty(request.FolderName))
                    return Json(new { success = false, message = "Folder name is required" });

                await _fileService.CreateFolderAsync(userId, request.FolderName, request.ParentFolderId);

                return Json(new { success = true, message = "Folder created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Delete File
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteFileRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _fileService.DeleteFileAsync(request.FileId, userId);

                return Json(new { success = true, message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Delete Folder
        [HttpPost]
        public async Task<IActionResult> DeleteFolder([FromBody] DeleteFolderRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                await _fileService.DeleteFolderAsync(request.FolderId, userId);

                return Json(new { success = true, message = "Folder deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Download File
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Auth");

                var client = _supabase.GetClient();
                var allFiles = await client.From<FileModel>().Get();
                var file = allFiles.Models.FirstOrDefault(f => f.Id == id && f.UserId == userId);

                if (file == null)
                    return NotFound();

                var fileBytes = await _supabase.DownloadFileAsync(file.FilePath, userId);

                return File(fileBytes, "application/octet-stream", file.FileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error downloading file: {ex.Message}";
                return RedirectToAction("Manager");
            }
        }

        // Search Files
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "User not authenticated" });

                var files = await _fileService.SearchFilesAsync(userId, query);

                return Json(new { success = true, files });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}