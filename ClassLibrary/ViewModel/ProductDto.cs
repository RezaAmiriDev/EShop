using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ModelLayer.ViewModel
{
    public class ProductDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? ProductCode { get; set; }
        public ProductType Type { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }
        public string? ShortDescription { get; set; }
        public Guid ShopId { get; set; }
        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }
    }
}
