using System;
namespace MedicalSystem.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string directory);
        Task<bool> DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
    }
}

