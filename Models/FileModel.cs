using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PersonalCloudDrive.Models
{
    [Table("files")]
    public class FileModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Column("file_type")]
        public string FileType { get; set; } = string.Empty;

        [Column("file_size")]
        public long FileSize { get; set; }

        [Column("uploaded_on")]
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        [Column("folder_id")]
        public int? FolderId { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("storage_url")]
        public string StorageUrl { get; set; } = string.Empty;
    }

    [Table("folders")]
    public class FolderModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("folder_name")]
        public string FolderName { get; set; } = string.Empty;

        [Column("parent_folder_id")]
        public int? ParentFolderId { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }

    [Table("shared_files")]
    public class ShareModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("file_id")]
        public int FileId { get; set; }

        [Column("share_token")]
        public string ShareToken { get; set; } = string.Empty;

        [Column("password")]
        public string? Password { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Column("download_count")]
        public int DownloadCount { get; set; } = 0;
    }

    public class DashboardViewModel
    {
        public decimal StorageUsed { get; set; }
        public decimal StorageLimit { get; set; }
        public int TotalFiles { get; set; }
        public int TotalFolders { get; set; }
        public List<FileModel> RecentFiles { get; set; } = new();
        public string UserName { get; set; } = string.Empty;
    }
}