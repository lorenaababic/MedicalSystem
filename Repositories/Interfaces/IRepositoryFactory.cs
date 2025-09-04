using System;
namespace MedicalSystem.Repositories.Interfaces
{
    public interface IRepositoryFactory
    {
        IPatientRepository CreatePatientRepository();
        IMedicalDocumentationRepository CreateMedicalDocumentationRepository();
        IExaminationRepository CreateExaminationRepository();
        IPrescriptionRepository CreatePrescriptionRepository();
        IMedicalImageRepository CreateMedicalImageRepository();
    }
}

