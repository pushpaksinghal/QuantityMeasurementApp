using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace QuantityMeasurementApp.RepositoryLayer.ConnectionFactory
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
