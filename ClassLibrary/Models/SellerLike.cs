using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class SellerLike
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
        public Guid? CustomerId { get; set; } // null => مهمان
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
