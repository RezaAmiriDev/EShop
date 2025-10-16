using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Address : BaseEntity
    {

        [Required(ErrorMessage = "شهر الزامی است")]
        [Display(Name = "City")]
        [MaxLength(20)]
        public string? City { get; set; }

        [Required(ErrorMessage = "استان الزامی است")]
        [Display(Name = "State")]
        [MaxLength(20)]
        public string? State { get; set; }

        [Required(ErrorMessage = "شماره تماس الزامی است")]
        [Display(Name = "Tell")]
        [MaxLength(20)]
        public string? Tellphone { get; set; }

        [Required(ErrorMessage = "آدرس الزامی است")]
        [Display(Name = "Adress Detail")]
        [MaxLength(50)]
        public string? AdressDetail { get; set; }

        public ICollection<Customer>? Customers { get; set; } // One-to-Many
    }
}
