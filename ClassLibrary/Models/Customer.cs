using ClassLibrary.Models;
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
    public class Customer : BaseEntity
    {

        [Required(ErrorMessage = "نام الزامی است")]
        [Display(Name = "Name")]
        [MaxLength(20)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [Display(Name = "Family")]
        [MaxLength(20)]
        public string? Family { get; set; }

        [Required(ErrorMessage = "تاریخ تولد الزامی است")]
        [Display(Name = "Birth")]
        [DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}")]
        public DateTime? Birth {  get; set; }

        [Required(ErrorMessage = "کد ملی الزامی است.")]
        [StringLength(10, ErrorMessage = "کد ملی باید دقیقاً ۱۰ کاراکتر باشد.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید عددی و ۱۰ رقمی باشد.")]
        [Display(Name = "NationalCode")]
        [MaxLength(10)]
        public string? NationalCode { get; set; }

        [Display(Name = "CreateDate")]
        [DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}")]
        public DateTime CreateDate { get; set; }

        //public DateTime? ModifiedDate { get; set; }
        //public string? ModifiedBy { get; set; }

        public Guid AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; } // Many-to-One

        public Guid ProductId { get; set; }

        public ICollection<Order> Sales { get; set; } = new List<Order>();

        public ICollection<Product>? products { get; set; } // Navigation property
       // public ICollection<ProductCustomer>? ProductCusromer { get; set; } // Many-to-Many
    }
}
