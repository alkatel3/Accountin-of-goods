namespace BLL.Interfaces
{
    public interface IProvider<T>
    {
         void Deliver(int Count, T entity);
    }
}
