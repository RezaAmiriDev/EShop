using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Repository
{
    public interface IAdressRepository : IRepository<Address>
    {
      Task<Address> SaveAndReturnId(Address address); 
    }
}
