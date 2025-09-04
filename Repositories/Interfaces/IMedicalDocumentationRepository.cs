using System;
using MedicalSystem.Models;

namespace MedicalSystem.Repositories.Interfaces
{
    public interface IMedicalDocumentationRepository
    {
        Task<IEnumerable<MedicalDocumentation>> GetAllAsync();
        Task<MedicalDocumentation?> GetByIdAsync(long id);
        Task<IEnumerable<MedicalDocumentation>> GetByPatientIdAsync(long patientId);
        Task<MedicalDocumentation> CreateAsync(MedicalDocumentation medicalDocumentation);
        Task<MedicalDocumentation> UpdateAsync(MedicalDocumentation medicalDocumentation);
        Task<bool> DeleteAsync(long id);
    }
}

