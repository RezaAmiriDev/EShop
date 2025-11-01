using DataLayer.EnumHellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ApiResult
{
    public class ServiceResult
    {
        public ServiceResult(ResponseStatus status , string? message = null)
        {
            Status = status;

            Message = string.IsNullOrWhiteSpace(message)
                ? EnumExtention.GetEnumDescription(status)
                : message!;
        }
        public string Message { get; set; } = string.Empty;
        public ResponseStatus Status { get; set; }

        /// اختیاری: ساخت سریع نتیجه موفق
        public static ServiceResult Ok(string? message = null) => new ServiceResult(ResponseStatus.Success, message);

        /// اختیاری: ساخت نتیجه خطا
        public static ServiceResult Error(string? message = null) => new ServiceResult(ResponseStatus.ServerError, message);
    }

    public class ServiceResultByData<T>
    {
        public ServiceResultByData(ResponseStatus status, string? message, T data)
        {
          
            Status = status;
            Message = string.IsNullOrWhiteSpace(message)
                ? EnumExtention.GetEnumDescription(status)
                : message!;
              
            Data = data;
        }
        public string Message { get; set; } = string.Empty;
        public ResponseStatus Status { get; set; }
        public T Data { get; set; } = default!;
        public static ServiceResultByData<T> Success(T data, string? message = null) => new ServiceResultByData<T>(ResponseStatus.Success, message, data);
        public static ServiceResultByData<T> Fail(string? message = null) => new ServiceResultByData<T>(ResponseStatus.ServerError, message, default!);

    }

}
