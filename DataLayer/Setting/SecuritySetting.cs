using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Setting
{
    public sealed class SecuritySetting
    {
        public string? SecretKey { get; set; }
        public string? Encryptkey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int NotBefore { get; set; }
        public int Expires { get; set; }
        public string? Scheme { get; set; }
        public string? HeaderName { get; set; }
        public string? BearerFormat { get; set; }
        public string? DescriptionScheme { get; set; }
    }
}
