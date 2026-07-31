using AutoMapper;
using ClassLibrary;
using ClassLibrary.Repository;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;
        public CartService(ICartRepository cartRepository, IProductRepository productRepository,ICustomerRepository customerRepository ,IRepository<CartItem> cartItemRepo, IMapper mapper, ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _cartItemRepository = cartItemRepo;
        }

        // ========== متد کمکی: اطمینان از وجود مشتری (برای مهمان) ==========
        private async Task<Customer> EnsureCustomerExistsAsync(Guid customerId, CancellationToken token)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, token);
            if(customer == null)
            {
                customer = new Customer
                {
                    Id = customerId,
                    Name = "مهمان",
                    Family = "",
                    UserId = null
                };
                await _customerRepository.AddAsync(customer, token);
                _logger.LogInformation("Customer {CustomerId} created as guest.", customerId);
            }
            return customer;
        }

        // ========== متد کمکی: دریافت CustomerId از کاربر لاگین ==========
        private async Task<Guid?> GetCustomerIdFromUserAsync(ClaimsPrincipal user)
        {
            if(user?.Identity?.IsAuthenticated != true) return null;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId)) return null;

            var customer = await _customerRepository.Table.FirstOrDefaultAsync(c => c.UserId == userId);
            return customer?.Id;
        }

        // ========== متد عمومی: تبدیل شناسه ورودی به Guid ==========
        public async Task<Guid?> ResolveCustomerIdAsync(string userId, ClaimsPrincipal? user = null)
        {
            if(user?.Identity?.IsAuthenticated == true)
            {
                var customerId = await GetCustomerIdFromUserAsync(user);
                if(customerId.HasValue) return customerId;
            }

            if(Guid.TryParse(userId, out var guestId)) return guestId;
            return null;
        }

        public async Task<CartDto> GetCartAsync(Guid userId, CancellationToken token)
        {
            try
            {
                var cart = await _cartRepository.TableNoTracking
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CustomerId == userId, token);
                if (cart == null)
                {
                    return new CartDto
                    {
                        CustomerId = userId,
                        Items = new List<CartItemDto>(),
                        TotalCount = 0,
                        TotalPrice = 0,
                    };
                }

                var dto = _mapper.Map<CartDto>(cart);
                // محاسبهٔ مجموع قیمت و تعداد
                dto.TotalPrice = cart.Items.Sum(i => i.Price * i.Count);
                dto.TotalCount = cart.Items.Sum(i => i.Count);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user {UserId} ", userId);
                throw;
            }
        }

        public async Task<ServiceResultByData<int>> GetCartCountAsync(Guid userId, CancellationToken token)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return new ServiceResultByData<int>(ResponseStatus.NotFound, "UserId is empty", 0);
                }
                var count = await _cartRepository.GetCartItemCountAsync(userId, token);
                return ServiceResultByData<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count for user {UserId}", userId);
                return ServiceResultByData<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> AddToCartAsync(AddToCartDto dto, CancellationToken token)
        {
            try
            {
                if (dto.CustomerId == null || dto.ProductId == null || dto.Count <= 0)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "Invalid input data");
                }

                var userId = dto.CustomerId.Value;
                var productId = dto.ProductId.Value;

                // بررسی وجود محصول
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                // دریافت سبد کاربر با آیتم‌های آن
                var cart = await _cartRepository.GetCartWithItemsAsync(userId, token);
                if (cart == null)
                {
                    // ایجاد سبد جدید
                    cart = new Cart { CustomerId = userId };
                    cart = await _cartRepository.InsertAndReturnAsync(cart, token);
                }

                // جستجوی آیتم تکراری
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Count += dto.Count;
                    existingItem.Price = product.Price;

                    var updateResult = await _cartItemRepository.UpdateAsync(existingItem);
                    if (updateResult.Status != ResponseStatus.Success)
                        return updateResult;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Count = dto.Count,
                        Price = product.Price
                    };

                    var addResult = await _cartItemRepository.AddAsync(newItem, token);
                    if (addResult.Status != ResponseStatus.Success)
                        return addResult;
                }

                return new ServiceResult(ResponseStatus.Success, "Item added to cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", dto?.CustomerId);
                return new ServiceResult(ResponseStatus.BadRequest, ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveFromCartAsync(Guid userId, Guid productId, CancellationToken token)
        {
            try
            {
                if (userId == Guid.Empty || productId == Guid.Empty)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "Invalid identifiers");
                }
                // get all the cart
                var cart = await _cartRepository.GetCartWithItemsAsync(userId, token);
                if (cart == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "Cart not found");
                }
                // then get the item of the cart here
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "Item not found in cart");
                }

                var deleteResult = await _cartItemRepository.DeleteAsync(item, token);
                return deleteResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart for user {UserId}", userId);
                return new ServiceResult(ResponseStatus.BadRequest, ex.Message);
            }
        }

        public async Task<ServiceResult> ClearCartAsync(Guid userId, CancellationToken token)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "UserId is empty");
                }

                await _cartRepository.ClearCartAsync(userId, token);
                return new ServiceResult(ResponseStatus.Success, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return new ServiceResult(ResponseStatus.BadRequest, ex.Message);
            }
        }
    }
}

