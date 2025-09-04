using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using MedicalSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalImageController : ControllerBase
    {
        private readonly IMedicalImageService _medicalImageService;
        private readonly IWebHostEnvironment _environment;

        public MedicalImageController(IMedicalImageService medicalImageService, IWebHostEnvironment environment)
        {
            _medicalImageService = medicalImageService;
            _environment = environment;
        }

        [HttpPost("upload/{examinationId}")]
        public async Task<ActionResult<MedicalImage>> UploadMedicalImage(long examinationId, IFormFile file)
        {
            try
            {
                var uploadedImage = await _medicalImageService.UploadImageAsync(examinationId, file);
                return CreatedAtAction(nameof(GetMedicalImage), new { id = uploadedImage.Id }, uploadedImage);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalImage>> GetMedicalImage(long id)
        {
            try
            {
                var repositoryFactory = HttpContext.RequestServices.GetRequiredService<IRepositoryFactory>();
                var imageRepository = repositoryFactory.CreateMedicalImageRepository();

                var image = await imageRepository.GetByIdAsync(id);
                if (image == null)
                {
                    return NotFound();
                }
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("examination/{examinationId}")]
        public async Task<ActionResult<IEnumerable<MedicalImage>>> GetImagesByExamination(long examinationId)
        {
            try
            {
                var images = await _medicalImageService.GetImagesByExaminationAsync(examinationId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadMedicalImage(long id)
        {
            try
            {
                var repositoryFactory = HttpContext.RequestServices.GetRequiredService<IRepositoryFactory>();
                var imageRepository = repositoryFactory.CreateMedicalImageRepository();

                var image = await imageRepository.GetByIdAsync(id);
                if (image == null)
                {
                    return NotFound();
                }

                var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");
                var fullPath = Path.Combine(uploadsPath, image.FilePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound("File not found on server");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                var contentType = GetContentType(image.FileType);

                return File(fileBytes, contentType, image.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMedicalImage(long id)
        {
            try
            {
                var success = await _medicalImageService.DeleteImageAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private static string GetContentType(string fileType)
        {
            return fileType.ToUpperInvariant() switch
            {
                "JPG" or "JPEG" => "image/jpeg",
                "PNG" => "image/png",
                "GIF" => "image/gif",
                "BMP" => "image/bmp",
                "TIFF" => "image/tiff",
                "WEBP" => "image/webp",
                "PDF" => "application/pdf",
                "DCM" => "application/dicom",
                _ => "application/octet-stream"
            };
        }
    }
}

