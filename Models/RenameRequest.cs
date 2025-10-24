namespace PersonalCloudDrive.Controllers.Models
{
    public class RenameRequest
    {
        public int ItemId { get; set; }
        public string NewName { get; set; } = string.Empty;
        public bool IsFolder { get; set; }
    }
}