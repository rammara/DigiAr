using System.ComponentModel.DataAnnotations;

namespace Mnemosyne.DataModels
{
    public class ApiKey
    {
        [Required]
        [MaxLength(100)]
        public string? Key { get; set; }
        [Required]
        public DateTime Expires { get; set; }
    } // class ApiKey
} // namespace
