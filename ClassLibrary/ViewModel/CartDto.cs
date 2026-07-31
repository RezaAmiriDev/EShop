using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class CartDto
    {
        public Guid? Id { get; set; }

        public Guid CustomerId { get; set; }

        public List<CartItemDto> Items { get; set; } = new();

        public decimal TotalPrice { get; set; }

        public int TotalCount { get; set; }
    }
}
