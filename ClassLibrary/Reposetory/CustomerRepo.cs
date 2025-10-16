using ClassLibrary.Models;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using DataLayer.EnumHellper;
using ModelLayer.Reposetotry;


namespace ClassLibrary.Repository
{
    public class CustomerRepo : Repos<Customer> , ICustomerRepository
    {
        public CustomerRepo(MobiContext mobiContext) : base(mobiContext) { }
      
        //public bool Add(Customer Model)
        //{
        //    try {
        //        _mobiContext.Customers.Add(Model);
        //        _mobiContext.SaveChanges();
        //        return true;
        //    }catch(Exception ex)
        //    {
        //        _logger.LogError(ex, "خطا در افزودن مشتری");
        //        return false;
        //    }
        //} 
        //public bool Delete(int Id)
        //{
        //   try
        //    {
        //        var data = this.FindById(Id);
        //        if (data == null)
        //            return false;
        //        _mobiContext.Customers.Remove(data);
        //        _mobiContext.SaveChanges();
        //        return true;
        //    } catch(Exception ex)
        //    {
        //        _logger.LogError(ex,"THERE IS ERR IN DELETE OF CUSTOMER");
        //        throw;
        //    }
        //}
        //public bool Update(Customer Model)
        //{
        //  try
        //    {
        //        _mobiContext.Customers.Update(Model);
        //        _mobiContext.SaveChanges();
        //        return true;
        //    }catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "UPDATE HAVE ISSUE IN ADRESS");
        //         throw;
        //    }
        //}
        //public Customer FindById(int Id)
        //{
        //    return _mobiContext.Customers.Find(Id);
        //}
        //public IEnumerable<Customer> FindAll()
        //{
        //    return _mobiContext.Customers.ToList();
        //}
        //public void save()
        //{
        //    _mobiContext.SaveChanges();
        //}
       
        public async Task<Customer> GetByIdentificationCode(string IdentificationCode)
        {
            try
            {
                return await Table.Where(w => w.NationalCode == IdentificationCode).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Customer> SaveAndReturnModel(Customer customer)
        {
            try
            {
                var ID = Guid.NewGuid();
                customer.Id = ID;
                await Entities.AddAsync(customer);
                await _mobiContext.SaveChangesAsync();
                return customer;
            }
            catch (Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }
        }
    //    public async Task<List<Customer>> Search(string searchTerm)
    //    {
    //        try
    //        {
    //            var query = _mobiContext.Customers.AsQueryable();
    //            if (string.IsNullOrEmpty(searchTerm))
    //            {
    //                query = query.Where(d => d.NationalCode == searchTerm);
    //            }
                 
    //            return await query.ToListAsync();
    //        }
    //        catch (Exception)
    //        {
    //           throw;
    //        }
    //    }
    //    public async Task<PaginatedList<Customer>> GetCustomersAsync(PaginatedList<Customer> paginated)
    //    { 
    //        try
    //        {
    //            IQueryable<Customer> query = _mobiContext.Customers;
    //            var totalCustomers = await _mobiContext.Customers.CountAsync();  // تعداد کل مشتریان
    //            // Get the total count of customers
    //            var count = await query.CountAsync();
    //            // Get the paginated list of customers
    //            var customers = await query.OrderByDescending(a => a.Id)
    //                .Skip((paginated.PageIndex -1) * paginated.PageSize) // Skip to the desired page
    //                .Take(paginated.PageSize).Select(c => new Customer
    //                {
    //                    Id = c.Id,
    //                    Name = c.Name,
    //                    Family = c.Family,
    //                    NationalCode = c.NationalCode,
    //                    Birth = c.Birth,
    //                }).ToListAsync();
    //            // Return the paginated list
    //            return new PaginatedList<Customer>(customers , totalCustomers , paginated.PageIndex ,paginated.PageSize);
    //        }catch (Exception)
    //        {
    //            throw;
    //        }
    //    }
    //
    }
}
