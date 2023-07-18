using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;

namespace StoreApi.DAL
{
    public class ProductRepository : IRepository<Product,ProductDTO>, IDisposable
    {

        private readonly StoreDbContext context;

        public ProductRepository(StoreDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
        public async Task<List<Product?>> GetAll()
        {
            return await context.Products.ToListAsync();
        }
        public async Task<Product?> GetById(int id)
        {
            Product? product = await context.Products.FindAsync(id);
            if (product != null)
            {
                return product;
            }
            return null;
        }
        public async Task<Product?> Insert(ProductDTO request)
        {
            try
            {
                Product product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Category = request.Category,
                    CategoryId  = request.CategoryId,
                    Price = request.Price,
                };
                await context.Products.AddAsync(product);
                await Save();
                return product;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }
        public async Task<bool> Delete(int id)
        {
            Product? product = await context.Products.FindAsync(id);
            if(product != null) {
               context.Products.Remove(product);
                await Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<Product?> Update(int id, ProductDTO request)
        {
            try
            {
                Product? _product = await context.Products.FindAsync(id);
                if(_product != null)
                {
                    _product.Name = request.Name;
                    _product.Description = request.Description;
                    _product.Price = request.Price;
                    _product.Category = request.Category;

                    context.Products.Update(_product);
                    await Save();
                    return _product;
                }
                else
                {
                    return null;
                }   
            }catch(Exception ex)
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
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

    }
}
