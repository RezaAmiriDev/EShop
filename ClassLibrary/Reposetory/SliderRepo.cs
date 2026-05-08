using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.Reposetotry;


namespace ModelLayer.Reposetory
{
    public class SliderRepo : Repos<SliderImage> , ISliderRepository
    {
        public SliderRepo(MobiContext context) : base(context) { }

        public async Task<List<SliderImage>> GetActiveSliderAsync(CancellationToken ct = default)
        {
            return await TableNoTracking
            .Where(s => s.IsActive) // فرض میکنم چنین فیلدی دارید
            .OrderBy(s => s.Order)
            .ToListAsync(ct);
        }
    }
}
