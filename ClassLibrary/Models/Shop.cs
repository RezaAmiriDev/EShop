using ClassLibrary;
using DataLayer.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class Shop : BaseEntity
    {
        public string? ShopName { get; set; }
        public string? Description { get; set; }
        public string? ShopCode { get; set; }
        public int LikesCount { get; set; } = 0;

        [Display(Name = "Image file")]
        public string? ImagePath { get; set; }
        [NotMapped] // این ویژگی را در دیتابیس ذخیره نکنید
        public IFormFile? ImageFile { get; set; }
        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        public ICollection<Product>? products { get; set; }
        public ICollection<SellerLike>? Likes { get; set; }
    }
}
