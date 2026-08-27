using System.ComponentModel.DataAnnotations;

namespace EShope.ViewModels
{
    public class UserViewModel
    {

        public string? Id { get; set; }

        [Required(ErrorMessage = "نام کاربری اجباری است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "رمز عبور حداقل ۶ کاراکتر")]
        [Display(Name = "رمز عبور")]
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "ایمیل نامعتبر")]
        [Display(Name = "ایمیل")]
        public string? Email { get; set; }

        [Display(Name = "نقش")]
        public string? CurrentRole { get; set; }

        [Display(Name = "نقش")]
        public string Role { get; set; }
    }
}