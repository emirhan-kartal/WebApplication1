using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _dbContext;

    public FileRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<WebApplication1.Models.File>> GetFilesByUserIdAsync(int userId)
    {
        return await _dbContext.Files
            .Where(f => f.Id == userId)
            .ToListAsync();
    }

    public async Task<WebApplication1.Models.File> GetFileByIdAsync(int fileId)
    {
        return await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId);
    }

    public async Task AddFileAsync(WebApplication1.Models.File file)
    {
        await _dbContext.Files.AddAsync(file);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFileAsync(int fileId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateFileAsync(WebApplication1.Models.File file)
    {
        _dbContext.Files.Update(file);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<WebApplication1.Models.File>> GetAllFilesAsync()
    {
        return await _dbContext.Files
            .ToListAsync();
    }


}
