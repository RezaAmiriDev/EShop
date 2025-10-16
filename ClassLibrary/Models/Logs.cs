using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class Logs : BaseEntity
    {
        public LogType LogType { get; set; }
        private DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Message { get; set; }
        public string? UserId { get; set; }
        //[ForeignKey(nameof(UserId))]
        //  public User User { get; set; }
    }


    public enum LogType
    {
        [Description("ورود به سایت")]
        Login = 1,
        [Description("خروج از سایت")]
        Logout = 2,
        [Description("ارسال پیامک ورود")]
        SendExpiredContractSms = 3,

        [Description("ثبت مشتری")]
        CreateCustomer = 11,
        [Description("ویرایش مشتری")]
        UpdateCustomer = 12,
        [Description("حذف مشتری")]
        DeleteCustomer = 13,

        [Description("ثبت کاربر")]
        CreateUser = 21,
        [Description("ویرایش کاربر")]
        UpdateUser = 22,
        [Description("حذف کاربر")]
        DeleteUser = 23,

        [Description("ثبت محصول")]
        CreateProduct = 40,
        [Description("ویرایش محصول")]
        UpdateProduct = 41,
        [Description("حذف محصول")]
        DeleteProduct = 42,

        [Description("تغییر شماره تماس")]
        ChngeNumber = 61,
        [Description("ویرایش جزءیات آدرس")]
        ChangeAddress = 62,
        [Description("ویرایش شهر")]
        ChangeCity = 63,
        

    }

}
