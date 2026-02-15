using ModelLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Interface
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<List<TopProductDto>> GetTopProductAsync(int top = 6);
        Task<List<DailyTransactionDto>> GetDailyTransactionAsync(int days = 15);
        Task<List<CityOrderDto>> GetTopCitiesByOrdersAsync(int top = 6);
    }
}
