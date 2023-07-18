using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;

namespace StoreApi.DAL
{
    public class UserRepository : IRepository<User,UserDTO>, IDisposable
    {
        private readonly StoreDbContext context;
        public UserRepository(StoreDbContext dbContext)
        {
            context = dbContext;
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
        public async Task<List<User?>> GetAll()
        {
            return await context.Users.Include(o => o.Orders).ThenInclude(p => p.Products).ToListAsync();
        }
        public async Task<User?> GetById(int id)
        {
            User? user = await context.Users.Include(o => o.Orders).ThenInclude(p => p.Products).FirstOrDefaultAsync(i => i.Id == id);
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public async Task<User?> Insert(UserDTO request)
        {
            try
            {
                if (request != null && context.Users.FirstOrDefault(user => user.Email == request.Email) == null)
                {
                    string passwordHash
                        = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    User newUser = new User
                    {
                        Name = request.Name,
                        Surname = request.Surname,
                        Email = request.Email,
                        Password = passwordHash,
                    };
                    await context.Users.AddAsync(newUser);
                    await Save();
                    return newUser;
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
            User? user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<User?> Update(int id, UserDTO request)
        {
            try
            {
                User? _user = await context.Users.FindAsync(id);
                if (_user != null)
                {
                    _user.Name = request.Name;
                    _user.Surname = request.Surname;
                    _user.Email = request.Email;
                    _user.Password = request.Password;

                    context.Users.Update(_user);
                    Save();
                    return _user;
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
