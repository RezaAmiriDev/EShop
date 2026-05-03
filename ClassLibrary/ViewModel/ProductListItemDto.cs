using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? ProductCode { get; set; }
        public string? ImagePath { get; set; }
        public decimal Price { get; set; }
        public string? ShopName { get; set; }
        public double AverageRating { get; set; }
        public string? ShortDescription { get; set; }
    }
}
