using System.ComponentModel.DataAnnotations;

namespace ContactApi.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;
    }
}
