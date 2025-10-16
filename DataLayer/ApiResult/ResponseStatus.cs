using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ApiResult
{
    public enum ResponseStatus
    {
        [Description("عملیات  مورد نظر با موفقیت انجام شد ")]
        Success = 1,
        [Description("!اطلاعات وارد شده  نا معتبر است")]
        BadRequest = 2,
        [Description("اطلاعاتی جهت نمایش یافت نشد! ")]
        NotFound = 3,
        [Description("در سیستم خطایی رخ داد! مجددا تلاش کنید ")]
        ServerError = 4,
        [Description("کاربری با مشخصات وارد شده وجود ندارد  ")]
        NotFoundUser = 5,
        [Description("اطلاعات وارد شده تکراری است ")]
        ExistProperty = 6,
        [Description("اعتبار  توکن به پایان رسید  ")]
        UnAuthorize = 401,
        [Description("به دلیل استفاده امکان حذف وجود ندارد")]
        CanNotDelete = 8,
        [Description("مجوز ورود به این قسمت را ندارد! ")]
        Forbidden = 403,
    }
}
