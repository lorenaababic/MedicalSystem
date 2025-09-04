using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;

namespace MedicalSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        // Lazy repositories - kreiraju se tek kad su potrebni
        private readonly Lazy<IPatientRepository> _patientRepository;
        private readonly Lazy<IMedicalDocumentationRepository> _medicalDocumentationRepository;
        private readonly Lazy<IExaminationRepository> _examinationRepository;
        private readonly Lazy<IPrescriptionRepository> _prescriptionRepository;

        public PatientService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;

            // ISPRAVKA: Uklonjen * operator
            _patientRepository = new Lazy<IPatientRepository>(() => _repositoryFactory.CreatePatientRepository());
            _medicalDocumentationRepository = new Lazy<IMedicalDocumentationRepository>(() => _repositoryFactory.CreateMedicalDocumentationRepository());
            _examinationRepository = new Lazy<IExaminationRepository>(() => _repositoryFactory.CreateExaminationRepository());
            _prescriptionRepository = new Lazy<IPrescriptionRepository>(() => _repositoryFactory.CreatePrescriptionRepository());
        }

        public async Task<PatientDetailsDto> GetPatientWithDetailsAsync(long patientId)
        {
            // Dohvaćanje pacijenta
            var patient = await _patientRepository.Value.GetByIdAsync(patientId);
            if (patient == null)
            {
                throw new ArgumentException($"Patient with ID {patientId} not found");
            }

            // Za sada vraćamo prazan rezultat jer ostali repository-ji možda nisu implementirani
            // Lazy loading - dohvaćaju se podaci tek kad su potrebni
            try
            {
                var medicalHistory = await _medicalDocumentationRepository.Value.GetByPatientIdAsync(patientId);
                var examinations = await _examinationRepository.Value.GetByPatientIdAsync(patientId);
                var prescriptions = await _prescriptionRepository.Value.GetByPatientIdAsync(patientId);

                return new PatientDetailsDto
                {
                    Patient = patient,
                    MedicalHistory = medicalHistory.ToList(),
                    Examinations = examinations.ToList(),
                    Prescriptions = prescriptions.ToList()
                };
            }
            catch (Exception)
            {
                // Ako ostali repository-ji nisu implementirani, vraćamo samo osnovne podatke
                return new PatientDetailsDto
                {
                    Patient = patient,
                    MedicalHistory = new List<MedicalDocumentation>(),
                    Examinations = new List<Examination>(),
                    Prescriptions = new List<Prescription>()
                };
            }
        }

        public async Task<byte[]> ExportPatientsToCSVAsync()
        {
            var patients = await _patientRepository.Value.GetAllAsync();
            var csvContent = new System.Text.StringBuilder();

            // CSV Header - dodajem PatientNumber
            csvContent.AppendLine("ID,FirstName,LastName,OIB,Gender,DateOfBirth,CreatedAt,PatientNumber");

            // CSV Data
            foreach (var patient in patients)
            {
                csvContent.AppendLine($"{patient.Id},{patient.FirstName},{patient.LastName},{patient.Oib},{patient.Gender},{patient.DateOfBirth:yyyy-MM-dd},{patient.CreatedAt:yyyy-MM-dd HH:mm:ss},{patient.PatientNumber}");
            }

            return System.Text.Encoding.UTF8.GetBytes(csvContent.ToString());
        }
    }
}