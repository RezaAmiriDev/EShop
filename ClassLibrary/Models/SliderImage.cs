using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    // Models/SliderImage.cs
    public class SliderImage : BaseEntity
    {
        public string? ImagePath { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int Order { get; set; } = 0;  // برای نمایش ترتیب 
    }

}
