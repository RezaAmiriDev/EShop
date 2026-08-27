using DataLayer.Base;
using System.ComponentModel.DataAnnotations;


namespace ClassLibrary
{
    public class Address : BaseEntity
    {

        [Required(ErrorMessage = "شهر الزامی است")]
        [Display(Name = "City")]
        [MaxLength(50)]
        public string? City { get; set; }

        [Required(ErrorMessage = "استان الزامی است")]
        [Display(Name = "State")]
        [MaxLength(100)]
        public string? State { get; set; }

        [Required(ErrorMessage = "شماره تماس الزامی است")]
        [Display(Name = "Tell")]
        [MaxLength(20)]
        public string? Tellphone { get; set; }

        [Required(ErrorMessage = "آدرس الزامی است")]
        [Display(Name = "Adress Detail")]
        [MaxLength(100)]
        public string? AdressDetail { get; set; }

        public ICollection<Customer>? Customers { get; set; } // One-to-Many
    }
}
