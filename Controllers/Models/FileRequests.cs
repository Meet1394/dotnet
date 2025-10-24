namespace PersonalCloudDrive.Controllers.Models;

public class CreateFolderRequest
{
    public required string FolderName { get; set; }
    public int? ParentFolderId { get; set; }
}

public class DeleteFileRequest
{
    public required int FileId { get; set; }
}

public class DeleteFolderRequest
{
    public required int FolderId { get; set; }
}