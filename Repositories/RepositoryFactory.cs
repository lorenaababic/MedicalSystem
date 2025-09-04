using System;
using MedicalSystem.Repositories.Interfaces;

namespace MedicalSystem.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly string _connectionString;

        public RepositoryFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public IPatientRepository CreatePatientRepository()
        {
            return new PatientRepository(_connectionString);
        }

        public IMedicalDocumentationRepository CreateMedicalDocumentationRepository()
        {
            return new MedicalDocumentationRepository(_connectionString);
        }

        public IExaminationRepository CreateExaminationRepository()
        {
            return new ExaminationRepository(_connectionString);
        }

        public IPrescriptionRepository CreatePrescriptionRepository()
        {
            return new PrescriptionRepository(_connectionString);
        }

        public IMedicalImageRepository CreateMedicalImageRepository()
        {
            return new MedicalImageRepository(_connectionString);
        }
    }
}

