using ClassLibrary;
using DataLayer.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ModelLayer.Models
{
    public class Shop : BaseEntity
    {
        public string? ShopName { get; set; }
        public string? Description { get; set; }
        public string? ShopCode { get; set; }
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;
        public int? NumberOfproducts { get; set; }

        [Display(Name = "Image file")]
        public string? ImagePath { get; set; }
        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        public ICollection<Product>? products { get; set; }
        public ICollection<SellerLike>? Likes { get; set; }
    }
}
