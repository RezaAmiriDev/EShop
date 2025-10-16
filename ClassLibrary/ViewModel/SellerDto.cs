using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class SellerDto
    {
        public Guid? Id { get; set; }
        public string? SellerName { get; set; }
        public string? SellerDescription { get; set; }
        public string? NationalCode { get; set; }
        public Guid AddressId { get; set; }
        // برای خلاصه آدرس/تعداد محصولات می‌توان اطلاعات اضافه کرد
        public int? ProductsCount { get; set; }
    }
}
