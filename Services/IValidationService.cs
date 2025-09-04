using System;
using MedicalSystem.Models;

namespace MedicalSystem.Services
{
    public interface IValidationService
    {
        Task<ValidationResult> ValidatePatientAsync(Patient patient);
        Task<ValidationResult> ValidateExaminationAsync(Examination examination);
        Task<ValidationResult> ValidatePrescriptionAsync(Prescription prescription);
    }
}

