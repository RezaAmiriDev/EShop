using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
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
    public class CartRepository : Repos<Cart>, ICartRepository
    {
        private readonly MobiContext _context;

        public CartRepository(MobiContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Cart?> GetUserCartAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Table.FirstOrDefaultAsync(x => x.CustomerId == userId, cancellationToken);
        }

        public async Task<Cart?> GetCartWithItemsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Table
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.CustomerId == userId, cancellationToken);
        }

        public async Task<int> GetCartItemCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cart = await Table
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.CustomerId == userId, cancellationToken);

            if (cart == null)
                return 0;

            return cart.Items.Sum(x => x.Count);
        }

        public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cart = await GetCartWithItemsAsync(userId, cancellationToken);

            if(cart == null)
                return;

            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }
    
    }
}
