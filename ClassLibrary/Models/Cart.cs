using ClassLibrary;
using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class Cart : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
