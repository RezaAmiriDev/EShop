using ClassLibrary.Models;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using DataLayer.EnumHellper;
using ModelLayer.Reposetotry;


namespace ClassLibrary.Repository
{
    public class CustomerRepo : Repos<Customer>, ICustomerRepository
    {
        public CustomerRepo(MobiContext mobiContext) : base(mobiContext) { }

        public async Task<List<Customer>> SearchAsync(string searchTerm, CancellationToken token = default)
        {

            var query = TableNoTracking;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(c =>
                c.NationalCode!.Contains(searchTerm) ||
                (c.Name != null && c.Name.Contains(searchTerm)) ||
                (c.Family != null && c.Family.Contains(searchTerm)));
            }
            else
            {
                return new List<Customer>();
            }
            return await query.ToListAsync(token);
        }
    }
}
