using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace ModelLayer.ViewModel
{
    public class ShopDto
    {
        public ShopDto()
        {
            AddressDto = new AddressDto(); // این خط اضافه شود
        }
        public Guid? Id { get; set; }
        public string? ShopName { get; set; }
        public string? Description { get; set; }
        public string? ShopCode { get; set; }
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;
        public int? NumberOfproducts { get; set; }
        public string? ImagePath { get; set; }
        [JsonIgnore]
        public IFormFile? Avatar { get; set; }
        public string? SellerId { get; set; }
        public AddressDto? AddressDto { get; set; } 
    }
}
