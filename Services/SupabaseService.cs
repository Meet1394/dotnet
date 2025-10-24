using Supabase;
using Client = Supabase.Client;

namespace PersonalCloudDrive.Services
{
    public class SupabaseService
    {
        private readonly Client _client;
        private readonly string _bucketName;

        public SupabaseService(IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase URL not configured");
            var key = configuration["Supabase:Key"] ?? throw new ArgumentNullException("Supabase Key not configured");
            _bucketName = configuration["Supabase:BucketName"] ?? "cloud-drive-files";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false
            };

            _client = new Client(url, key, options);
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        public Client GetClient()
        {
            return _client;
        }

        public string GetBucketName()
        {
            return _bucketName;
        }

        // Upload file to Supabase Storage
        public async Task<string> UploadFileAsync(string userId, string fileName, byte[] fileContent)
        {
            try
            {
                // The file path structure should include the userId for RLS: {userId}/{Guid}_{fileName}
                var filePath = $"{userId}/{Guid.NewGuid()}_{fileName}";
                
                // IMPORTANT: The Upload method usually requires a separate authentication flow
                // if the bucket is private and requires the user's JWT. 
                // We assume the Supabase-csharp SDK handles the session for authenticated client.

                await _client.Storage
                    .From(_bucketName)
                    .Upload(fileContent, filePath);

                var publicUrl = _client.Storage
                    .From(_bucketName)
                    .GetPublicUrl(filePath);

                return publicUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

        // 🔄 UPDATED: Download file from Supabase Storage with userId for RLS
    public async Task<byte[]> DownloadFileAsync(string filePath, string? userId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("File path cannot be empty");

                // Ensure the path uses forward slashes
                filePath = filePath.Replace("\\", "/").TrimStart('/');

                // If a full public URL was stored, extract the relative path after the bucket name
                if (filePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var bucketMarker = $"/{_bucketName}/";
                    var idx = filePath.IndexOf(bucketMarker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                        filePath = filePath.Substring(idx + bucketMarker.Length);
                }

                // Assuming the file path saved in the DB is the full path required by Supabase Storage
                // The client used here must be capable of authenticating the request (e.g., via JWT)
                // The Supabase-csharp SDK often handles this implicitly if the client is set up 
                // with the user session token in a custom way.
                
                // NOTE: If the file is PRIVATE, you should use .Download() or .CreateSignedUrl()
                // .Download() should be sufficient if the client is authenticated via the JWT, 
                // allowing RLS to grant access based on the userId in the path.

                var options = new Supabase.Storage.TransformOptions();
                    byte[]? fileBytes = null;
                    Exception? lastEx = null;

                    // Try direct download first
                    try
                    {
                        fileBytes = await _client.Storage
                            .From(_bucketName)
                            .Download(filePath, options);
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex;
                    }

                    // If failed and we have a userId, try prefixing with userId (in case DB stored path without it)
                    if ((fileBytes == null || fileBytes.Length == 0) && !string.IsNullOrEmpty(userId))
                    {
                        try
                        {
                            var altPath = filePath;
                            if (!altPath.StartsWith(userId + "/", StringComparison.OrdinalIgnoreCase))
                                altPath = $"{userId}/{altPath}";

                            fileBytes = await _client.Storage
                                .From(_bucketName)
                                .Download(altPath, options);
                        }
                        catch (Exception ex)
                        {
                            lastEx = ex;
                        }
                    }

                    if (fileBytes == null || fileBytes.Length == 0)
                        throw lastEx ?? new Exception("File is empty or not found");

                return fileBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file: {ex.Message}");
            }
        }

        // Delete file from Supabase Storage
        public async Task DeleteFileAsync(string filePath, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("File path cannot be empty");

                // Ensure the path uses forward slashes
                filePath = filePath.Replace("\\", "/").TrimStart('/');

                // If a full public URL was passed, extract the relative path
                if (filePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var bucketMarker = $"/{_bucketName}/";
                    var idx = filePath.IndexOf(bucketMarker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                        filePath = filePath.Substring(idx + bucketMarker.Length);
                }

                // Ensure the file path starts with the userId for RLS
                if (!filePath.StartsWith(userId + "/", StringComparison.OrdinalIgnoreCase))
                    filePath = $"{userId}/{filePath}";

                await _client.Storage
                    .From(_bucketName)
                    .Remove(new List<string> { filePath });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}");
            }
        }

        // Get file URL
        public string GetFileUrl(string filePath)
        {
            // NOTE: If the bucket is PRIVATE, this method should return a SIGNED URL
            // For a simple public access, GetPublicUrl is fine, but defeats RLS security
            return _client.Storage
                .From(_bucketName)
                .GetPublicUrl(filePath);
        }
    }
}