using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace QuantityMeasurementApp.RepositoryLayer.ConnectionFactory
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
