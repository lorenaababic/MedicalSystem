using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;

namespace MedicalSystem.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ValidationService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<ValidationResult> ValidatePatientAsync(Patient patient)
        {
            var result = new ValidationResult();

            // Check if OIB already exists (for create/update scenarios)
            if (patient.Id == 0) // New patient
            {
                var existingPatient = await _repositoryFactory.CreatePatientRepository().GetByOibAsync(patient.Oib);
                if (existingPatient != null)
                {
                    result.Errors.Add("OIB already exists in the system");
                }
            }

            // Check age constraints
            var age = DateTime.Now.Year - patient.DateOfBirth.Year;
            if (patient.DateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age > 120)
            {
                result.Errors.Add("Patient age cannot exceed 120 years");
            }

            if (patient.DateOfBirth > DateTime.Now)
            {
                result.Errors.Add("Date of birth cannot be in the future");
            }

            return result;
        }

        public async Task<ValidationResult> ValidateExaminationAsync(Examination examination)
        {
            var result = new ValidationResult();

            // Check if patient exists
            var patient = await _repositoryFactory.CreatePatientRepository().GetByIdAsync(examination.PatientId);
            if (patient == null)
            {
                result.Errors.Add("Patient not found");
                return result;
            }

            // Check examination date/time constraints
            if (examination.ExaminationDate > DateTime.Now.AddDays(365))
            {
                result.Errors.Add("Examination cannot be scheduled more than 1 year in advance");
            }

            // Check for duplicate examinations on same day/time for same patient
            var existingExaminations = await _repositoryFactory.CreateExaminationRepository().GetByPatientIdAsync(examination.PatientId);
            var duplicateExamination = existingExaminations.FirstOrDefault(e =>
                e.Id != examination.Id &&
                e.ExaminationDate.Date == examination.ExaminationDate.Date &&
                e.ExaminationTime == examination.ExaminationTime);

            if (duplicateExamination != null)
            {
                result.Errors.Add("Patient already has an examination at this date and time");
            }

            return result;
        }

        public async Task<ValidationResult> ValidatePrescriptionAsync(Prescription prescription)
        {
            var result = new ValidationResult();

            // Check if patient exists
            var patient = await _repositoryFactory.CreatePatientRepository().GetByIdAsync(prescription.PatientId);
            if (patient == null)
            {
                result.Errors.Add("Patient not found");
                return result;
            }

            // Check if examination exists (if provided)
            if (prescription.ExaminationId.HasValue)
            {
                var examination = await _repositoryFactory.CreateExaminationRepository().GetByIdAsync(prescription.ExaminationId.Value);
                if (examination == null)
                {
                    result.Errors.Add("Examination not found");
                }
                else if (examination.PatientId != prescription.PatientId)
                {
                    result.Errors.Add("Examination does not belong to the specified patient");
                }
            }

            // Check issue date
            if (prescription.IssueDate > DateTime.Now)
            {
                result.Errors.Add("Issue date cannot be in the future");
            }

            return result;
        }
    }

    public class ValidationResult
    {
        public List<string> Errors { get; set; } = new();
        public bool IsValid => !Errors.Any();
    }
}