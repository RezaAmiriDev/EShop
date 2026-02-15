using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ModelLayer.ViewModel
{
    public class ProductDto
    {
        public Guid? Id { get; set; }
        public string? Brand { get; set; }
        public string? ProductCode { get; set; }
        public ProductType Type { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }

        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }
    }
}
