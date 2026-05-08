using ClassLibrary.Repository;
using ModelLayer.Models;

namespace ModelLayer.Interface
{
    public interface ISliderRepository : IRepository<SliderImage>
    {
       Task<List<SliderImage>> GetActiveSliderAsync(CancellationToken ct = default);
    }
}
