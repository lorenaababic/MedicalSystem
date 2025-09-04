using System;
using MedicalSystem.Models;

namespace MedicalSystem.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(long id);
        Task<Patient?> GetByOibAsync(string oib);
        Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(long id);
    }
}

