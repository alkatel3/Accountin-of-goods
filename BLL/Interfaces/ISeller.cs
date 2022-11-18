namespace BLL.Interfaces
{
    public interface ISeller<T>
    {
        void Sell(int Count, T entity);
    }
}
