using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;
using System.Security.Claims;

namespace StoreApi.DAL
{
    public class OrderRepository: IRepository<Order,OrderDTO>, IDisposable
    {
        private readonly StoreDbContext context;
        public OrderRepository(StoreDbContext dbContext)
        {
            context = dbContext;
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
        public async Task<List<Order?>> GetAll()
        {
            return await context.Orders.Include(p => p.Products).Include(u => u.User).ToListAsync();
        }
        public async Task<Order?> GetById(int id)
        {
            Order? order = await context.Orders.Include(p => p.Products).Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == id);
            if (order != null)
            {
                return order;
            }
            return null;
        }
        public async Task<Order?> Insert(OrderDTO request)
        {
            try
            {
                User? existingUser = await context.Users.FindAsync(request.UserID);
                if( existingUser != null)
                {
                    Order newOrder = new Order();
                    foreach (var product in request.Products)
                    {
                        // Obtener el producto existente por su ID
                        Product? existingProduct = await context.Products.FindAsync(product.Id);

                        if (existingProduct != null && existingUser != null)
                        {
                            newOrder.Products.Add(existingProduct);
                            newOrder.User = existingUser;
                            newOrder.UserId = existingUser.Id;
                        }
                    }
                    context.Orders.Add(newOrder);
                    await Save();
                    return newOrder;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }
        public async Task<bool> Delete(int id)
        {
            Order? order = await context.Orders.FindAsync(id);
            if (order != null)
            {
                context.Orders.Remove(order);
                await Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<Order?> Update(int id, OrderDTO request)
        {
            try
            {
                Order? order = await context.Orders.FindAsync(id);
                if (order != null)
                {
                    foreach (var productId in request.Products)
                    {
                        // Obtener el producto existente por su ID
                        var existingProduct = await context.Products.FindAsync(productId);
                        if (existingProduct != null)
                        {
                            order.Products.Add(existingProduct);
                        }
                    }
                    context.Orders.Update(order);
                    await Save();
                    return order;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Liberar recursos administrados
                    context.Dispose();
                }

                // Liberar recursos no administrados
                // ...

                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
    }
}
