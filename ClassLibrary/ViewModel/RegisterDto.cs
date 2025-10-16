using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نادرست است")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
