using StoreApi.Models;

namespace StoreApi.DAL
{
    public interface IRepository<T1,T2> : IDisposable where T1 : class where T2 : class
    {
        Task<List<T1?>> GetAll();
        Task<T1?> GetById(int id);
        Task<T1?> Insert(T2 request);
        Task<bool> Delete(int id);
        Task<T1?> Update(int id, T2 request);
        Task Save();
    }

}
