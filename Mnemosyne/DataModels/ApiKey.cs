using System.ComponentModel.DataAnnotations;

namespace Mnemosyne.DataModels
{
    public class ApiKey
    {
        [Key]
        [StringLength(100)]
        public string Key { get; set; }
        [Required]
        public DateTime Expires { get; set; }
    } // class ApiKey
} // namespace
