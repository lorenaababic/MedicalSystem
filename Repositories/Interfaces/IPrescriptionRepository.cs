using System;
using MedicalSystem.Models;

namespace MedicalSystem.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetAllAsync();
        Task<Prescription?> GetByIdAsync(long id);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(long patientId);
        Task<IEnumerable<Prescription>> GetByExaminationIdAsync(long examinationId);
        Task<Prescription> CreateAsync(Prescription prescription);
        Task<Prescription> UpdateAsync(Prescription prescription);
        Task<bool> DeleteAsync(long id);
    }
}

