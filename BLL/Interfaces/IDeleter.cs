namespace BLL.Interfaces
{
    public interface IDeleter<T>
    {
        void Delete(T entity);
    }
}
