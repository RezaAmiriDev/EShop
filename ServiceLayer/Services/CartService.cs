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
        private async Task<ServiceResultByData<Customer>> EnsureCustomerExistsAsync(Guid customerId, CancellationToken token)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, token);
            if (customer != null)
                return ServiceResultByData<Customer>.Success(customer);

            customer = new Customer
            {
                Id = customerId,
                Name = "مهمان",
                Family = "",
                UserId = null,
                CreateDate = DateTime.UtcNow,
            };

            var addResult = await _customerRepository.AddAsync(customer, token);
            if (addResult.Status != ResponseStatus.Success)
            {
                _logger.LogError("Failed to create guest customer {CustomerId}: {Message}", customerId, addResult.Message);
                return new ServiceResultByData<Customer>(addResult.Status, addResult.Message ?? "خطا در ایجاد مشتری", null!);
            }

            _logger.LogInformation("Customer {CustomerId} created as guest.", customerId);
            return ServiceResultByData<Customer>.Success(customer);
        }
        // ========== متد عمومی: تبدیل شناسه ورودی به Guid ==========
        public async Task<Guid?> ResolveCustomerIdAsync(string userId, ClaimsPrincipal? user = null)
        {
            if (user?.Identity?.IsAuthenticated == true)
            {
                var appUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(appUserId))
                {
                    var customer = await _customerRepository.Table.FirstOrDefaultAsync(c => c.UserId == appUserId);
                    if (customer != null) return customer.Id;

                    var newCustomer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        UserId = appUserId,
                        Name = "کاربر",
                        Family = "",
                        CreateDate = DateTime.UtcNow,
                    };

                    var addResult = await _customerRepository.AddAsync(newCustomer, CancellationToken.None);
                    if (addResult.Status != ResponseStatus.Success)
                    {
                        _logger.LogError("Failed to create customer for user {UserId}: {Message}", appUserId, addResult.Message);
                        return null; // ← دیگه با یک Id جعلی ادامه نمی‌ده
                    }

                    _logger.LogInformation("Customer created for user {UserId} with Id {CustomerId}", appUserId, newCustomer.Id);
                    return newCustomer.Id;
                }
            }

            if (Guid.TryParse(userId, out var guestId))
                return guestId;

            return null;
        }

        public async Task<CartDto> GetCartAsync(Guid userId, CancellationToken token)
        {
            try
            {
                await EnsureCustomerExistsAsync(userId, token);
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

        public async Task<ServiceResult> UpdateCartItemAsync(Guid customerId, Guid productId, int count, CancellationToken token)
        {
            try
            {
                if (customerId == Guid.Empty || productId == Guid.Empty)
                    return new ServiceResult(ResponseStatus.BadRequest, "Invalid identifiers");

                await EnsureCustomerExistsAsync(customerId, token);
                var cart = await _cartRepository.GetCartWithItemsAsync(customerId, token);
                if (cart == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "Cart not found");
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null) return new ServiceResult(ResponseStatus.NotFound, "Item not found in cart");

                if (count <= 0) return await _cartItemRepository.DeleteAsync(item, token);

                item.Count = count;
                return await _cartItemRepository.UpdateAsync(item);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item for user {UserId}", customerId);
                return new ServiceResult(ResponseStatus.BadRequest, ex.Message);
            }
        }

        public async Task<ServiceResultByData<int>> GetCartCountAsync(Guid userId, CancellationToken token)
        {
            try
            {
                await EnsureCustomerExistsAsync(userId, token);
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

        public async Task<ServiceResult> AddToCartAsync(Guid customerId, Guid productId, int count, CancellationToken token)
        {
            try
            {
                if (customerId == Guid.Empty || productId == Guid.Empty || count <= 0)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "Invalid input data");
                }

                var cusResult = await EnsureCustomerExistsAsync(customerId, token);
                if (cusResult.Status != ResponseStatus.Success)
                {
                    return new ServiceResult(cusResult.Status, cusResult.Message);
                }

                // بررسی وجود محصول
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                // دریافت سبد کاربر با آیتم‌های آن
                var cart = await _cartRepository.GetCartWithItemsAsync(customerId, token);
                if (cart == null)
                {
                    // ایجاد سبد جدید
                    cart = new Cart { CustomerId = customerId };
                    cart = await _cartRepository.InsertAndReturnAsync(cart, token);
                    if (cart == null)
                    {
                        _logger.LogError("Failed to create cart for customer {CustomerId}", customerId);
                        return new ServiceResult(ResponseStatus.BadRequest, "خطا در ایجاد سبد خرید");
                    }
                }

                // جستجوی آیتم تکراری
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Count += count;
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
                        Count = count,
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
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", customerId);
                return new ServiceResult(ResponseStatus.BadRequest, ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveFromCartAsync(Guid userId, Guid productId, CancellationToken token)
        {
            try
            {
                await EnsureCustomerExistsAsync(userId, token);
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
                await EnsureCustomerExistsAsync(userId, token);
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

