namespace WebApplication1.Repositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<Models.File>> GetFilesByUserIdAsync(int userId);
        Task<Models.File> GetFileByIdAsync(int fileId);
        Task AddFileAsync(Models.File file);
        Task DeleteFileAsync(int fileId);
        Task UpdateFileAsync(Models.File file);
        Task<IEnumerable<Models.File>> GetAllFilesAsync();
    }
}
