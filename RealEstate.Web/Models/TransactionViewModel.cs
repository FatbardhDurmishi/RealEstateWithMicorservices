using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Web.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }

        [StringLength(450)]
        public string? OwnerId { get; set; }

        [StringLength(450)]
        public string? BuyerId { get; set; }

        public int? PropertyId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime RentStartDate { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        [Required]
        public DateTime RentEndDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? RentPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public string TransactionType { get; set; } = null!;

        [NotMapped]
        public bool ShowButtons { get; set; }
    }
}