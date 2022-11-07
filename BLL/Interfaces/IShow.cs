namespace BLL.Interfaces
{
    public interface IShow<T>
    {
        List<T> GetAll();
        T GetCurrent(int id);
    }
}
