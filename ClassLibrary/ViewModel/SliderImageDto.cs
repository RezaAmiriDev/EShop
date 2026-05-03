
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ModelLayer.ViewModel
{
    public class SliderImageDto
    {
        public Guid? Id { get; set; }

        public string? ImagePath { get; set; }

        [JsonIgnore]
        public IFormFile? Slider { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
    }

}
