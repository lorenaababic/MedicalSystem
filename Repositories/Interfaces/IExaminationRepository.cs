using System;
using MedicalSystem.Models;

namespace MedicalSystem.Repositories.Interfaces
{
    public interface IExaminationRepository
    {
        Task<IEnumerable<Examination>> GetAllAsync();
        Task<Examination?> GetByIdAsync(long id);
        Task<IEnumerable<Examination>> GetByPatientIdAsync(long patientId);
        Task<IEnumerable<Examination>> GetByTypeAsync(string examinationType);
        Task<Examination> CreateAsync(Examination examination);
        Task<Examination> UpdateAsync(Examination examination);
        Task<bool> DeleteAsync(long id);
    }
}

