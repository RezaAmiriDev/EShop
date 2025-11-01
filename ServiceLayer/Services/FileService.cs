// ServiceLayer/Services/FileService.cs
using Microsoft.AspNetCore.Http;
using ModelLayer.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

public class FileService : IFileRepository
{
    public async Task<string?> SaveFileAsync(IFormFile file, string webRootPath, string folder)
    {
        if (file == null || file.Length == 0) return null;

        var folderPath = folder.Trim('/').Replace('/', Path.DirectorySeparatorChar);
        var uploads = Path.Combine(webRootPath ?? "wwwroot", folderPath);
        Directory.CreateDirectory(uploads);

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploads, fileName);

        await using var fs = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        // مسیر قابل استفاده در <img src="...">
        return $"/{folder.Trim('/')}/{fileName}";
    }

    public bool DeleteFile(string fileRelativePath, string webRootPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileRelativePath)) return false;
            var trimmed = fileRelativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(webRootPath ?? "wwwroot", trimmed);
            if (File.Exists(full)) File.Delete(full);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
