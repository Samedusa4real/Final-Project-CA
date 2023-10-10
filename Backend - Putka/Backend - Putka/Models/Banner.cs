using Backend___Putka.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend___Putka.Models
{
    public class Banner
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string TopText { get; set; }
        [MaxLength(25)]
        public string HeaderOne { get; set; }
        [MaxLength(25)]
        public string SaleText { get; set; }
        [MaxLength(25)]
        public string HeaderTwo { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [MaxLength(25)]
        public string ButtonText { get; set; }
        [MaxLength(250)]
        public string ButtonLink { get; set; }
        [Required]
        public bool IsSmall { get; set; }
        [Required]
        [MaxLength(250)]
        public string BackgroundImage { get; set; }
        [MaxFileSize(10)]
        [AllowedExtensions("image/jpeg", "image/png")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
