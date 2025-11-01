using ClassLibrary.Models;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.Reposetotry;


namespace ModelLayer.Reposetory
{
    public class ShopRepo : Repos<Shop> , IShopRepository
    {
        public ShopRepo(MobiContext context) : base(context)
        {
        }
    }
}
