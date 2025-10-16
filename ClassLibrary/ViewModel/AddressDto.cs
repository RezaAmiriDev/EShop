using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class AddressDto
    {
        public Guid? Id { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Tellphone { get; set; }
        public string? AdressDetail { get; set; }
    }

}
