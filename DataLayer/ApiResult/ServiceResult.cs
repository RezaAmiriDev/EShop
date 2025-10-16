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
        public ServiceResult(ResponseStatus statuscode , string? message)
        {
            if(message == null || message == "")
            {
                Message  = EnumExtention.GetEnumDescription(statuscode);
            }
            else
            {
                Message = message;
            }
            Status = statuscode;
        }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }
    public class ServiceResultByData<Tdata>
    {
        public ServiceResultByData(ResponseStatus statuscode, string? message, Tdata data)
        {
            if (message == null || message == "")
            {
                Message = EnumExtention.GetEnumDescription(statuscode);
            }
            else
            {
                Message = message;
            }
            Status = statuscode;
            Data = data;
        }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
        public Tdata Data { get; set; }
    }   
   
}
