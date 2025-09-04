using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using MedicalSystem.Services.Interfaces;

namespace MedicalSystem.Services
{
    public class MedicalImageService : IMedicalImageService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IFileStorageService _fileStorageService;
        private readonly Lazy<IMedicalImageRepository> _medicalImageRepository;

        private readonly HashSet<string> _allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".pdf", ".dcm"
        };

        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public MedicalImageService(IRepositoryFactory repositoryFactory, IFileStorageService fileStorageService)
        {
            _repositoryFactory = repositoryFactory;
            _fileStorageService = fileStorageService;
            _medicalImageRepository = new Lazy<IMedicalImageRepository>(() => _repositoryFactory.CreateMedicalImageRepository());
        }

        public async Task<MedicalImage> UploadImageAsync(long examinationId, IFormFile file)
        {
            // Validation
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum limit of {MaxFileSize / (1024 * 1024)}MB");
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"File type {fileExtension} is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
            }

            // Save file to storage
            var directory = $"medical-images/{examinationId}";
            var filePath = await _fileStorageService.SaveFileAsync(file, directory);

            // Create database record
            var medicalImage = new MedicalImage
            {
                ExaminationId = examinationId,
                FileName = file.FileName,
                FilePath = filePath,
                FileType = fileExtension.TrimStart('.').ToUpperInvariant(),
                FileSize = file.Length
            };

            return await _medicalImageRepository.Value.CreateAsync(medicalImage);
        }

        public async Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(long examinationId)
        {
            return await _medicalImageRepository.Value.GetByExaminationIdAsync(examinationId);
        }

        public async Task<bool> DeleteImageAsync(long imageId)
        {
            var image = await _medicalImageRepository.Value.GetByIdAsync(imageId);
            if (image == null)
            {
                return false;
            }

            // Delete file from storage
            await _fileStorageService.DeleteFileAsync(image.FilePath);

            // Delete database record
            return await _medicalImageRepository.Value.DeleteAsync(imageId);
        }
    }
}

