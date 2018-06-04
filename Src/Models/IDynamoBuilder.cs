namespace Src
{
    using System.Threading.Tasks;

    public interface IDynamoBuilder
    {
        Task BuildUserTable();
    }
}
