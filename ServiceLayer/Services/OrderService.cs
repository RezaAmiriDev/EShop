using ClassLibrary.Models;
using ClassLibrary.Repository;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using ModelLayer.ViewModel;
using ModelLayer.Models;
using Common.Pagination;

namespace ServiceLayer.Services
{
    public class OrderService
    {
        private readonly IOrderReposetory _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly MobiContext _context;

        public OrderService(
            IOrderReposetory orderRepository,
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            MobiContext context)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _context = context;
        }

        /// <summary>
        /// Create sale (transactional). Returns created Order as OrderDto.
        /// </summary>
        public async Task<ServiceResultByData<OrderDto>> CreateSaleAsync(Guid customerId, Guid productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                    return new ServiceResultByData<OrderDto>(ResponseStatus.BadRequest, "تعداد باید بزرگتر از صفر باشد.,", null!);

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    return new ServiceResultByData<OrderDto>(ResponseStatus.NotFound, "مشتری یافت نشد.", null!);

                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return new ServiceResultByData<OrderDto>(ResponseStatus.NotFound,"محصول یافت نشد.", null!);

                // read price/name via reflection only if needed (keeps compatibility with different product models)
                var priceProp = product.GetType().GetProperty("Price");
                var nameProp = product.GetType().GetProperty("Name") ?? product.GetType().GetProperty("name");
                decimal unitPrice = priceProp != null ? Convert.ToDecimal(priceProp.GetValue(product) ?? 0m) : 0m;
                string productName = nameProp != null ? nameProp.GetValue(product)?.ToString() ?? "" : "";

                var stockProp = product.GetType().GetProperty("Stock");
                if (stockProp != null)
                {
                    var stockVal = (int?)stockProp.GetValue(product) ?? 0;
                    if (stockVal < quantity)
                        return new ServiceResultByData<OrderDto>(ResponseStatus.BadRequest, "موجودی محصول کافی نیست.", null!);
                }

                // start transaction
                await using var tx = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Add sale using repository (OrderRepo handles Entities.AddAsync and SaveChanges)
                    var order = await _orderRepository.AddSaleAsync(customerId, productId, quantity);

                    // ensure some fields
                    order.UnitPrice = order.UnitPrice == 0m ? unitPrice : order.UnitPrice;
                    order.ProductNameSnapshot = order.ProductNameSnapshot ?? productName;
                    order.TotalPrice = order.TotalPrice == 0m ? (order.UnitPrice * order.Quantity) : order.TotalPrice;
                    order.OrderNumber ??= $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

                    // update product stock if present
                    if (stockProp != null)
                    {
                        var stockVal = (int?)stockProp.GetValue(product) ?? 0;
                        stockProp.SetValue(product, stockVal - quantity);

                        var updateResult = await _productRepository.UpdateAsync(product);
                        // if your UpdateAsync returns ServiceResult-like, check status; otherwise assume non-null success
                        if (updateResult == null || (updateResult is ServiceResult sr && sr.Status != ResponseStatus.Success))
                        {
                            await tx.RollbackAsync();
                            return new ServiceResultByData<OrderDto>(ResponseStatus.ServerError,"خطا در به‌روزرسانی موجودی محصول.", null!);
                        }
                    }

                    // update order if needed and commit
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    // prepare dto
                    var dto = new OrderDto
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        ProductId = order.ProductId,
                        ProductName = order.ProductNameSnapshot ?? productName,
                        CustomerId = order.CustomerId,
                        CustomerName = customer != null ? $"{customer.Name} {customer.Family}".Trim() : null,
                        NationalCode = customer != null ? customer.NationalCode : null,
                        UnitPrice = order.UnitPrice == 0m ? (decimal?)null : order.UnitPrice,
                        Quantity = order.Quantity > 0 ? (int?)order.Quantity : null,
                        ShippingCost = order.ShippingCost,
                        TotalPrice = order.TotalPrice,
                        AmountPaid = 0m,
                  //      PaymentStatus = order.PaymentStatus.ToString(),
                        Currency = order.Currency,
                        Status = order.Status.ToString(),
                        SaleDate = order.SaleDate
                    };

                    return ServiceResultByData<OrderDto>.Success(dto);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    return new ServiceResultByData<OrderDto>(ResponseStatus.ServerError, "خطا در ثبت فروش: " + ex.Message, null!);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResultByData<OrderDto>(ResponseStatus.ServerError, ex.Message, null!);
            }
        }

        /// <summary>
        /// Get sales grouped as one record per order for a customer.
        /// </summary>
        public async Task<ServiceResultByData<List<OrderDto>>> GetSalesByCustomerAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    return new ServiceResultByData<List<OrderDto>>(ResponseStatus.NotFound, "مشتری یافت نشد.", null!);

                // Use repository method
                var sales = await _orderRepository.GetSalesByCustomerAsync(customerId);
                if (sales == null || !sales.Any())
                    return ServiceResultByData<List<OrderDto>>.Success(new List<OrderDto>());

                // gather order ids for payments aggregation
                var orderIds = sales.Select(s => s.Id).ToList();

                var payments = await _context.Payments
                    .Where(p => orderIds.Contains(p.OrderId) && p.Status == ModelLayer.Models.PaymentStatus.Paid)
                    .GroupBy(p => p.OrderId)
                    .Select(g => new { OrderId = g.Key, Paid = g.Sum(x => x.Amount), LastPay = g.Max(x => x.PaymentDate) })
                    .ToDictionaryAsync(x => x.OrderId, x => x);

                var dtos = sales.Select(s =>
                {
                    payments.TryGetValue(s.Id, out var payInfo);
                    decimal? paid = payInfo?.Paid ?? 0m;
                    DateTime? lastPay = payInfo?.LastPay;

                    var required = (s.TotalPrice) + (s.ShippingCost);
                    string paymentStatus = paid >= required ? "Paid" : (paid > 0 ? "PartiallyPaid" : "NotPaid");

                    return new OrderDto
                    {
                        Id = s.Id,
                        OrderNumber = s.OrderNumber,
                        ProductId = s.ProductId == Guid.Empty ? (Guid?)null : s.ProductId,
                        ProductName = s.ProductNameSnapshot,
                        CustomerId = s.CustomerId,
                        CustomerName = s.Customer != null ? $"{s.Customer.Name} {s.Customer.Family}".Trim() : null,
                        NationalCode = s.Customer != null ? s.Customer.NationalCode : null,
                        UnitPrice = s.UnitPrice == 0m ? (decimal?)null : s.UnitPrice,
                        Quantity = s.Quantity > 0 ? s.Quantity : 1,
                        ShippingCost = s.ShippingCost,
                        TotalPrice = s.TotalPrice,
                        AmountPaid = paid,
                        PaymentStatus = paymentStatus,
                        Currency = s.Currency,
                        Status = s.Status.ToString(),
                        SaleDate = s.SaleDate
                    };
                }).ToList();

                 return ServiceResultByData<List<OrderDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return new ServiceResultByData<List<OrderDto>>(ResponseStatus.ServerError,ex.Message, null!);
            }
        }

        /// <summary>
        /// Get sales for a product (one record per order that includes product)
        /// </summary>
        public async Task<ServiceResultByData<List<OrderDto>>> GetSalesByProductAsync(Guid productId)
        {
            try
            {
                var sales = await _orderRepository.GetSalesByProductAsync(productId);
                if (sales == null || !sales.Any())
                    return ServiceResultByData<List<OrderDto>>.Success(new List<OrderDto>());

                var orderIds = sales.Select(s => s.Id).ToList();

                var payments = await _context.Payments
                    .Where(p => orderIds.Contains(p.OrderId) && p.Status == ModelLayer.Models.PaymentStatus.Paid)
                    .GroupBy(p => p.OrderId)
                    .Select(g => new { OrderId = g.Key, Paid = g.Sum(x => x.Amount), LastPay = g.Max(x => x.PaymentDate) })
                    .ToDictionaryAsync(x => x.OrderId, x => x);

                var dtos = sales.Select(s =>
                {
                    payments.TryGetValue(s.Id, out var payInfo);
                    decimal? paid = payInfo?.Paid ?? 0m;

                    var required = (s.TotalPrice) + (s.ShippingCost);
                    string paymentStatus = paid >= required ? "Paid" : (paid > 0 ? "PartiallyPaid" : "NotPaid");

                    return new OrderDto
                    {
                        Id = s.Id,
                        OrderNumber = s.OrderNumber,
                        ProductId = s.ProductId == Guid.Empty ? (Guid?)null : s.ProductId,
                        ProductName = s.ProductNameSnapshot,
                        CustomerId = s.CustomerId,
                        CustomerName = s.Customer != null ? $"{s.Customer.Name} {s.Customer.Family}".Trim() : null,
                        NationalCode = s.Customer != null ? s.Customer.NationalCode : null,
                        UnitPrice = s.UnitPrice == 0m ? (decimal?)null : s.UnitPrice,
                        Quantity = s.Quantity > 0 ? s.Quantity : 1,
                        ShippingCost = s.ShippingCost,
                        TotalPrice = s.TotalPrice,
                        AmountPaid = paid,
                        PaymentStatus = paymentStatus,
                        Currency = s.Currency,
                        Status = s.Status.ToString(),
                        SaleDate = s.SaleDate
                    };
                }).ToList();

                return ServiceResultByData<List<OrderDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return new ServiceResultByData<List<OrderDto>>(ResponseStatus.ServerError, ex.Message, null!);
            }
        }

        /// <summary>
        /// Get total sales amount
        /// </summary>
        public async Task<ServiceResultByData<decimal>> GetTotalSalesAsync()
        {
            try
            {
                var total = await _orderRepository.GetTotalSalesAsync();
                return ServiceResultByData<decimal>.Success(total);
            }
            catch (Exception ex)
            {
                return ServiceResultByData<decimal>.Fail(ex.Message);
            }
        }
   
        public async Task<ServiceResultByData<PagedResponse<List<PaymentDto>>>> GetRecentPaymentsAsync(PagedRequest<PaymentDto> pagedRequest)
        {
            try
            {
                var query = _context.Payments.AsNoTracking()
                    .Include(p => p.Order).ThenInclude(o => o.Customer)
                    .OrderByDescending(p => p.PaymentDate ?? p.CreatedAt);

                var totalRecords = await query.CountAsync();
                var payments = await query.Skip(pagedRequest.StartIndex)
                    .Take(pagedRequest.PageSize)
                    .Select(p => new PaymentDto
                    {
                        Id = p.Id,                      // <- از BaseEntity.Id
                        OrderId = p.OrderId,
                        CustomerName = p.Order != null && p.Order.Customer != null
                        ? (p.Order.Customer.Name + " " + p.Order.Customer.Family).Trim() : null,
                        Amount = p.Amount,
                        Currency = p.Currency,
                        Status = p.Status.ToString(),
                        PaymentDate = p.PaymentDate,
                        Provider = p.Provider.ToString(),
                        ProviderTransactionId = p.ProviderTransactionId,
                        TransactionReference = p.TransactionReference,
                        IsVerified = p.IsVerified,
                        Note = p.Note
                    }).ToListAsync();

                var response = new PagedResponse<List<PaymentDto>>(pagedRequest.PageNumber, pagedRequest.PageSize,totalRecords, payments);
                return ServiceResultByData<PagedResponse<List<PaymentDto>>>.Success(response);
            }
            catch(Exception ex)
            {
                return new ServiceResultByData<PagedResponse<List<PaymentDto>>>(ResponseStatus.ServerError, ex.Message, null!);
            }
        }
    }
}
