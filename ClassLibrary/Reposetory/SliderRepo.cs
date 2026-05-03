using ClassLibrary.Models;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.Reposetotry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Reposetory
{
    public class SliderRepo : Repos<SliderImage> , ISliderRepository
    {
        public SliderRepo(MobiContext context) : base(context) { }

        public async Task<List<SliderImage>> GetActiveSliderAsync()
        {
          //  return await TableNoTracking
           // .Where(s => s.IsActive) // فرض میکنم چنین فیلدی دارید
          //  .OrderBy(s => s.Order)
          //  .ToListAsync();
        }
    }
}
