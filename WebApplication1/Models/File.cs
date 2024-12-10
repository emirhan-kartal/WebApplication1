namespace WebApplication1.Models
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public long FileSize { get; set; } // Size in bytes
        public int UploadedByUserId { get; set; }

        public User UploadedByUser { get; set; }
    }
}
