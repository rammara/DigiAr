using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mnemosyne.DataModels
{
    public class Quote
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = Constants.NOT_AVAILABLE;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
    } // class Quote
} // namespace
