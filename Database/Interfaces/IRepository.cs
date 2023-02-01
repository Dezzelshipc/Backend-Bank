namespace Database.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetItem(int id);
        void Create(T item);
        bool Update(T item);
        bool Delete(int id);
        void Save();
    }
}
