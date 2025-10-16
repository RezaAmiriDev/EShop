using ClassLibrary;
using ClassLibrary.Models;
using ClassLibrary.Repository;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using ModelLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class OrderService
    {
        private readonly IOrderReposetory _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly MobiContext _context;

        public OrderService(IOrderReposetory orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository, MobiContext context)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _context = context;
        }

        /// ایجاد فروش (transactional): افزودن رکورد Sale و کم‌کردن موجودی محصول
        public async Task<ServiceResult> CreateSaleAsync(Guid customerId, Guid productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "تعداد باید بزرگتر از صفر باشد.");
                }

                // بررسی وجود مشتری
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "مشتری یافت نشد.");
                }

                // دریافت محصول (با ردیابی تا بتوانیم آن را به‌روزرسانی کنیم)
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "محصول یافت نشد.");
                }

                var priceProp = product.GetType().GetProperty("Price");
                var nameProp = product.GetType().GetProperty("name");
                decimal unitPrice = priceProp != null ? Convert.ToDecimal(priceProp.GetValue(product) ?? 0m) : 0m;
                string productName = nameProp != null ? nameProp.GetValue(product)?.ToString() ?? "" : "" ;

                var StockProperty = product.GetType().GetProperty("Stock");
                if (StockProperty != null)
                {
                    var stockVal = (int?)StockProperty.GetValue(product) ?? 0;
                    if (stockVal < quantity)
                    {
                        return new ServiceResult(ResponseStatus.BadRequest, "موجودی محصول کافی نیست.");
                    }
                }
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var order = await _orderRepository.AddSaleAsync(customerId, productId, quantity);

                    order.UnitPrice = order.UnitPrice == 0m ? unitPrice : order.UnitPrice;
                    order.ProductNameSnapshot = order.ProductNameSnapshot ?? productName;
                    order.TotalPrice = order.TotalPrice == 0m ? order.UnitPrice * order.Quantity : order.TotalPrice;
                    order.OrderNumber ??= $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

                    if (StockProperty != null)
                    {
                        var stockVal = (int?)StockProperty.GetValue(product) ?? 0;
                        StockProperty.SetValue(product, stockVal - quantity);

                        var updateResult = await _productRepository.UpdateAsync(product);
                        if (updateResult == null || updateResult.Status != ResponseStatus.Success)
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResult(ResponseStatus.ServerError, "خطا در به‌روزرسانی موجودی محصول.");
                        }
                    }

                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    // 3) کامیت تراکنش
                    await transaction.CommitAsync();

                    // ساخت SaleDto برای بازگرداندن به لایهٔ API
                    var dto = new OrderDto
                    {
                        Id = order.Id,
                        ProductId = order.ProductId,
                        CustomerId = order.CustomerId,
                        ProductNameSnapshot = order.ProductNameSnapshot,
                        UnitPrice = order.UnitPrice,
                        Quantity = order.Quantity,
                        ShippingCost = order.ShippingCost,
                        TotalPrice = order.TotalPrice,
                        AmountPaid = 0m,
                        PaymentStatus = order.PaymentStatus.ToString(),
                        OrderNumber = order.OrderNumber,
                        SaleDate = order.SaleDate,
                        CustomerName = $"{customer.Name} {customer.Family}".Trim()
                    };

                    return new ServiceResult(ResponseStatus.Success, null);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult(ResponseStatus.ServerError, "خطا در ثبت فروش.");
                }
            }
            catch
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }

        }

        /// گرفتن لیست فروش‌ها برای یک مشتری (برگرداندن DTO)
        public async Task<ServiceResult> GetSalesByCustomerAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "مشتری یافت نشد.");
                }

                var dto = from o in _context.Orders
                          where o.CustomerId == customerId
                          orderby o.SaleDate descending
                          select new OrderDto
                          {
                              Id = o.Id,
                              ProductId = o.ProductId,
                              CustomerId = o.CustomerId,
                              OrderNumber = o.OrderNumber,
                              ProductNameSnapshot = o.ProductNameSnapshot,
                              UnitPrice = o.UnitPrice,
                              Quantity = o.Quantity,
                              ShippingCost = o.ShippingCost,
                              TotalPrice = o.TotalPrice,
                              AmountPaid = _context.Payments
                                     .Where(p => p.OrderId == o.Id && p.Status == ModelLayer.Models.PaymentStatus.Succeeded)
                                     .Select(p => (decimal?)p.Amount)
                                     .Sum() ?? 0m,
                              PaymentStatus = o.PaymentStatus.ToString(),
                              Currency = o.Currency,
                              Status = o.Status.ToString(),
                              SaleDate = o.SaleDate,
                              // optional helper fields (if navigation available)
                              ProductName = o.ProductNameSnapshot ?? (o.Product != null ? o.Product.Name : null),
                              CustomerName = o.Customer != null ? $"{o.Customer.Name} {o.Customer.Family}".Trim() : null

                          };

                var list = await dto.ToListAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        /// گرفتن لیست فروش‌ها برای یک محصول
        public async Task<ServiceResult> GetSalesByProductAsync(Guid productId)
        {
            try
            {
                var sales = await _orderRepository.GetSalesByProductAsync(productId);
                if(sales == null | !sales.Any())
                {
                    return new ServiceResult(ResponseStatus.Success, null);

                }

                // جمع آیدی سفارش‌ها
                var orderId = sales.Select(s => s.Id).ToList();

                var paymentsGrouped = await _context.Payments
                    .Where(p => orderId.Contains(p.OrderId) && p.Status == ModelLayer.Models.PaymentStatus.Succeeded)
                    .GroupBy(p => p.OrderId)
                    .Select(g => new { orderId = g.Key, TotalPaid = g.Sum(x => x.Amount) })
                    .ToDictionaryAsync(x => x.orderId, x => x.TotalPaid);

                var dtos = sales.Select(s =>
                {
                    paymentsGrouped.TryGetValue(s.Id, out var paid);

                    var required = s.TotalPrice + s.ShippingCost;
                    string paymentStatus;
                    if (paid >= required) paymentStatus = "Paid";
                    else if (paid > 0) paymentStatus = "PartiallyPaid";
                    else paymentStatus = "NotPaid";

                    return new OrderDto
                    {
                        Id = s.Id,
                        ProductId = s.ProductId,
                        CustomerId = s.CustomerId,
                        OrderNumber = s.OrderNumber,
                        ProductNameSnapshot = s.ProductNameSnapshot,
                        UnitPrice = s.UnitPrice,
                        Quantity = s.Quantity,
                        ShippingCost = s.ShippingCost,
                        TotalPrice = s.TotalPrice,
                        AmountPaid = paid,
                        PaymentStatus = paymentStatus,
                        Currency = s.Currency,
                        Status = s.Status.ToString(),
                        SaleDate = s.SaleDate,
                        ProductName = s.ProductNameSnapshot ?? (s.Product != null ? s.Product.GetType().GetProperty("Name")?.GetValue(s.Product)?.ToString() : null),
                        CustomerName = s.Customer != null ? $"{s.Customer.Name} {s.Customer.Family}".Trim() : null
                    };
                }).ToList();

                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {
                // _logger?.LogError(ex, "GetSalesByProductAsync error");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        /// مجموع فروش (جمع TotalPrice)
        public async Task<ServiceResult> GetTotalSalesAsync()
        {
            try
            {
                var total = await _orderRepository.GetTotalSalesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {
                // _logger?.LogError(ex, "GetTotalSalesAsync error");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

    }
}
