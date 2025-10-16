using ClassLibrary.Models;
using DataLayer.ApiResult;
using DataLayer.EnumHellper;
using ModelLayer.Reposetotry;

namespace ClassLibrary.Repository
{
    public class AddressRepo : Repos<Address>, IAdressRepository
    {
        public AddressRepo(MobiContext mobiContext) : base(mobiContext)
        {

        }
        public async Task<Address> SaveAndReturnId(Address address)
        {
            try
            {
                var ID = Guid.NewGuid();
                address.Id = ID;
                await Entities.AddAsync(address);
                await _mobiContext.SaveChangesAsync();
                return address;
            }
            catch (Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }

        }

    }
}
