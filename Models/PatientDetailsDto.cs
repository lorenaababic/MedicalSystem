using System;
namespace MedicalSystem.Models
{
    public class PatientDetailsDto
    {
        public Patient Patient { get; set; }
        public List<MedicalDocumentation> MedicalHistory { get; set; } = new();
        public List<Examination> Examinations { get; set; } = new();
        public List<Prescription> Prescriptions { get; set; } = new();
    }
}

