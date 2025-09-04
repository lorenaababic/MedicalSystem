using System;
using MedicalSystem.Models;

namespace MedicalSystem.Repositories.Interfaces
{
    public interface IMedicalImageRepository
    {
        Task<IEnumerable<MedicalImage>> GetAllAsync();
        Task<MedicalImage?> GetByIdAsync(long id);
        Task<IEnumerable<MedicalImage>> GetByExaminationIdAsync(long examinationId);
        Task<MedicalImage> CreateAsync(MedicalImage medicalImage);
        Task<MedicalImage> UpdateAsync(MedicalImage medicalImage);
        Task<bool> DeleteAsync(long id);
    }
}

