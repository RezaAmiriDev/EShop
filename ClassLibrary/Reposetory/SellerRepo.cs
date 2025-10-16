using ClassLibrary.Models;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.Reposetotry;


namespace ModelLayer.Reposetory
{
    public class SellerRepo : Repos<Shop> , ISellerRepository
    {
        public SellerRepo(MobiContext context) : base(context)
        {
        }
    }
}
