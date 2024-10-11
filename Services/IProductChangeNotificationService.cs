using TableDependency.SqlClient.Base.EventArgs;
using WebApplication2.DTO;

namespace WebApplication2.Services
{
    public interface IProductChangeNotificationService
    {
        //void OnProductChanged(object sender, RecordChangedEventArgs<Product> e);
        void SubscribeTableDependency(string connectionString);
    }
}
