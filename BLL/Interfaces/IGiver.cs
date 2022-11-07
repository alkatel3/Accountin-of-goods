namespace BLL.Interfaces
{
    public interface IGiver<T>
    {
        List<T> GetAll();
        T GetCurrent(int id);
    }
}
