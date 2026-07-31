using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class AddToCartDto
    {
        public Guid? CustomerId { get; set; }

        public Guid? ProductId { get; set; }

        public int Count { get; set; } = 1;
    }
}
