using Database.Models;

namespace Database.Interfaces
{
    public interface IRepository<T> where T : IModel
    {
        IEnumerable<T> GetAll();
        T? GetItem(int id);
        void Create(T item);
        bool Update(T item);
        bool Delete(int id);
        bool Delete(T item);
        void Save();
    }
}
