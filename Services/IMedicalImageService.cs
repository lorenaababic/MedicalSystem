using System;
using MedicalSystem.Models;

namespace MedicalSystem.Services
{
    public interface IMedicalImageService
    {
        Task<MedicalImage> UploadImageAsync(long examinationId, IFormFile file);
        Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(long examinationId);
        Task<bool> DeleteImageAsync(long imageId);
    }
}

