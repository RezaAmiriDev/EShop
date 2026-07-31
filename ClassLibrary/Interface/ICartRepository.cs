using ClassLibrary.Repository;
using DataLayer.ApiResult;
using ModelLayer.Models;


namespace ModelLayer.Interface
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken cancellation = default);
        Task<int> GetCartItemCountAsync(Guid userId, CancellationToken cancellation = default);
        Task<Cart?> GetCartWithItemsAsync(Guid userId, CancellationToken cancellation = default);
        Task ClearCartAsync(Guid userId, CancellationToken cancellation = default);
    }
}
