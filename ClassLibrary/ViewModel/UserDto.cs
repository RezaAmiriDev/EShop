using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    // DTO برای دریافت و ارسال داده در API
    public class UserDto
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } // در Create اجباری، در Update اختیاری
        public string Role { get; set; }
    }
}
