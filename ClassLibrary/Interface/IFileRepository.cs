using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Interface
{
    public interface IFileRepository
    {
        Task<string> SaveFileAsync(IFormFile file, string rootPath, string folder);
        bool DeleteFile(string filePath, string rootPath);
    }
}
