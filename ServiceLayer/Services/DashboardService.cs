using ModelLayer.Interface;
using ModelLayer.ViewModel;

namespace ServiceLayer.Services
{
    public class DashboardService
    {
        private readonly IDashboardRepository _repo;
        public DashboardService(IDashboardRepository repository)
        {
            _repo = repository;
        }

        public Task<DashboardSummaryDto> GetSummaryAsync() =>
            _repo.GetSummaryAsync();

        public Task<List<DailyTransactionDto>> GetDailyTransactionsAsync(int days = 15) =>
            _repo.GetDailyTransactionAsync(days);

        public Task<List<TopProductDto>> GetTopProductsAsync(int top = 6) =>
            _repo.GetTopProductAsync(top);

        public Task<List<CityOrderDto>> GetTopCitiesByOrdersAsync(int top = 6) =>
            _repo.GetTopCitiesByOrdersAsync(top);

        public async Task<DashboardDto> GetAllAsync(int top = 6, int days = 15)
        {
            var summary = await GetSummaryAsync();
            var topProduct = await GetTopProductsAsync(top);
            var daily = await GetDailyTransactionsAsync(days);

            return new DashboardDto
            {
                Summary = summary,
                TopProducts = topProduct,
                DailyTransactions = daily
            };
        }
       
    }
}
