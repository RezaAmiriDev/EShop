using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class CartItemDto
    {
        public Guid? Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public string ImagePath { get; set; } = "";

        public decimal Price { get; set; }

        public int Count { get; set; }

        public decimal TotalPrice => Price * Count;
    }
}
