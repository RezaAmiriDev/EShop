using ClassLibrary;
using Microsoft.AspNetCore.Identity;
using System;

namespace ModelLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        // ارتباط با مشتری
        public virtual Customer? Customer { get; set; }
    }
}
