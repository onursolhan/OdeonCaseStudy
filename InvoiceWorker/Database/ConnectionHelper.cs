using System.Data.SqlClient;

namespace InvoiceWorker.Database
{
    internal class ConnectionHelper
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=Odeon;User Id=sa;Password=very_hard_password;";

        public static SqlConnection CreateSqlConnection() => new SqlConnection(ConnectionString);
    }
}
