using System;
using System.Collections.Generic;

namespace PersonalCloudDrive.Models
{
    public class FileManagerViewModel
    {
        public string UserName { get; set; }
        public List<FileModel> Files { get; set; }
        public List<FolderModel> Folders { get; set; }
        public int? CurrentFolderId { get; set; }
        public string CurrentFolderPath { get; set; }
        public double StorageUsed { get; set; }
        public double StorageLimit { get; set; }
        
        // 🆕 NEW PROPERTY: Tracks the active filter (e.g., "image", "other", or null)
        public string? CurrentFilterType { get; set; } 
    }
}