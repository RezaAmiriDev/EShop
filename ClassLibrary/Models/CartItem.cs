using ClassLibrary;
using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class CartItem : BaseEntity
    {

        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        public Cart? Cart { get; set; }

        public Product? Product { get; set; }
    }
}
