using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Web.Models.Dtos
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int? BedRooms { get; set; }
        public int? BathRooms { get; set; }

        public decimal Area { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } = null!;

        public string State { get; set; } = null!;

        public string City { get; set; } = null!;

        public string StreetAddress { get; set; } = null!;

        public string CoverImageUrl { get; set; } = null!;
        public string TransactionType { get; set; } = null!;

        public string? UserId { get; set; }

        public int? PropertyTypeId { get; set; }

        public List<string> BlobUrls { get; set; } = new List<string>();

        public string CoverImageBlobUrl { get; set; } = null!;

        public bool ShowButtons = false;

        public string CurrentUserId { get; set; } = null!;
        public string CurrentUserRole { get; set; } = null!;
    }
}