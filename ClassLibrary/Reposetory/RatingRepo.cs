using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Interface;
using ModelLayer.Models;


namespace ModelLayer.Reposetory
{
    public class RatingRepo : IRatingRepository
    {
        private readonly MobiContext _context;
        public RatingRepo(MobiContext context) => _context = context;


        public async Task<double> GetAverageRatingForProductAsync(Guid productId)
        {
            return await _context.Ratings
                .Where(r => r.ProductId == productId && r.IsApproved)
                .AverageAsync(r => (double?)r.Review) ?? 0;
        }

        public async Task<bool> HasUserRatedProductAsync(Guid productId, Guid customerId)
        {
           return await _context.Ratings
                .AnyAsync(r => r.ProductId == productId && r.CustomerId == customerId);
        }

        public async Task AddOrUpdateRatingAsync(Guid productId, Guid customerId, int review, string? title = null, string? body = null)
        {
            var existing = await _context.Ratings
                 .FirstOrDefaultAsync(r => r.ProductId == productId && r.CustomerId == customerId);
            if (existing != null)
            {
                existing.Review = review;
                existing.Title = title;
                existing.Body = body;
                existing.CreatedAt = DateTime.UtcNow;
                existing.IsApproved = false;
            }
            else
            {
                var rating = new Rating
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CustomerId = customerId,
                    Review = review,
                    Title = title,
                    Body = body,
                    CreatedAt = DateTime.UtcNow,
                    IsApproved = false
                };
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Rating>> GetApprovedRatingsForProductAsync(Guid productId)
        {
           return await _context.Ratings
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
   
    }
}
