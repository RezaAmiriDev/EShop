using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.Hellper
{
    public static class JwtHelper
    {
        public static Dictionary<string, JsonElement> DecodeJwtPayload(string jwt)
        {
            var parts = jwt.Split('.');
            var payload = parts[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;
        }
    }
}
