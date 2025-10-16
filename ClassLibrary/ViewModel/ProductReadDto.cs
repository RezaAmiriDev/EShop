using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class ProductReadDto
    {
        public Guid? Id { get; set; }
        public string? Brand { get; set; }
        public ProductType Type { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }
    }
}
