using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.ViewModel
{
    public class ProductUpdateDto
    {
        public Guid Id { get; set; }
        [MaxLength(20)]
        public string? Brand { get; set; }
        public ProductType Type { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; } // optional
    }
}
