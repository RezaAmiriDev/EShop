using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EnumHellper
{
    public static class EnumExtention
    {
        public static string GetEnumDescription(Enum value)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString())!;
                try
                {
                    DescriptionAttribute[] attributes =
                        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0)
                        return attributes[0].Description;
                    else
                        return value.ToString();
                }
                catch
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "نا مشخص";
            }
        }
        public static List<EnumModel> GetEnumlist<TName>() where TName : Enum
        {
            var list = Enum.GetValues(typeof(TName)).Cast<TName>().ToList();
            List<EnumModel> listenums = new List<EnumModel>();
            foreach (var item in list)
            {
                EnumModel model = new EnumModel();
                model.Id = Convert.ToInt32(item);
                model.Name = item.ToString();
                model.Description = GetEnumDescription(item);
                listenums.Add(model);
            }
            return listenums;
        }
    }
}
