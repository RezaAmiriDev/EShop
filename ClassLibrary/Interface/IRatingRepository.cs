using ModelLayer.Models;



namespace ModelLayer.Interface
{
    public interface IRatingRepository
    {
        Task<double> GetAverageRatingForProductAsync(Guid productId);
        Task<bool> HasUserRatedProductAsync(Guid productId, Guid customerId);
        Task AddOrUpdateRatingAsync(Guid productId, Guid customerId, int review, string? title = null, string? body = null);
        Task<List<Rating>> GetApprovedRatingsForProductAsync(Guid productId);
    }
}
