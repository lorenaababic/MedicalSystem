using System;
using MedicalSystem.Models;

namespace MedicalSystem.Services
{
    public interface IPatientService
    {
        Task<PatientDetailsDto> GetPatientWithDetailsAsync(long patientId);
        Task<byte[]> ExportPatientsToCSVAsync();
    }
}

