using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTool.Library.Services
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BenchmarkDB;Integrated Security=True;Encrypt=False";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
