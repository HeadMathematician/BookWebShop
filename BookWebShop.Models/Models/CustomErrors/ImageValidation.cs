using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookWebShop.Models.Models.CustomErrors
{
    public class ImageValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile file = value as IFormFile;

            if (file == null)
            {
                return new ValidationResult("Invalid file type.");
            }

            if (!IsImage(file))
            {
                return new ValidationResult("Only image files are allowed.");
            }

            return ValidationResult.Success;
        }

        private bool IsImage(IFormFile file)
        {
            string[] allowedTypes = { "image/jpeg", "image/png" };

            return allowedTypes.Contains(file.ContentType);
        }
    }
}
