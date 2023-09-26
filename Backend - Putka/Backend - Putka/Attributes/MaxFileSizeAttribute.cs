using System.ComponentModel.DataAnnotations;

namespace Backend___Putka.Attributes
{
    public class MaxFileSizeAttribute:ValidationAttribute
    {
        private readonly int _maxSizeInMegabytes;

        public MaxFileSizeAttribute(int maxSizeInMegabytes)
        {
            _maxSizeInMegabytes = maxSizeInMegabytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            long maxSizeInBytes = _maxSizeInMegabytes * 1024 * 1024;

            if (value is IFormFile)
            {
                IFormFile file = value as IFormFile;

                if (file.Length > maxSizeInBytes)
                {
                    return new ValidationResult($"FileSize must be less than or equal to {_maxSizeInMegabytes} megabytes.");
                }
            }
            else if (value is List<IFormFile>)
            {
                List<IFormFile> files = value as List<IFormFile>;
                foreach (var file in files)
                {
                    if (file.Length > maxSizeInBytes)
                    {
                        return new ValidationResult($"FileSize must be less than or equal to {_maxSizeInMegabytes} megabytes.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
