using Npgsql;
using QuantityMeasurementApp.RepositoryLayer.ConnectionFactory;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.RepositoryLayer.Utility
{
    public class QuantityHistoryRepository : IQuantityHistoryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public QuantityHistoryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void AddRecord(QuantityHistoryRecord record)
        {
            using NpgsqlConnection connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = @"
                INSERT INTO QuantityHistory
                (
                    Category,
                    OperationType,
                    FirstValue,
                    FirstUnit,
                    SecondValue,
                    SecondUnit,
                    TargetUnit,
                    ResultValue,
                    ResultUnit
                )
                VALUES
                (
                    @Category,
                    @OperationType,
                    @FirstValue,
                    @FirstUnit,
                    @SecondValue,
                    @SecondUnit,
                    @TargetUnit,
                    @ResultValue,
                    @ResultUnit
                )";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", record.Category);
            command.Parameters.AddWithValue("@OperationType", record.OperationType);
            command.Parameters.AddWithValue("@FirstValue", record.FirstValue);
            command.Parameters.AddWithValue("@FirstUnit", record.FirstUnit);
            command.Parameters.AddWithValue("@SecondValue", (object?)record.SecondValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@SecondUnit", (object?)record.SecondUnit ?? DBNull.Value);
            command.Parameters.AddWithValue("@TargetUnit", (object?)record.TargetUnit ?? DBNull.Value);
            command.Parameters.AddWithValue("@ResultValue", record.ResultValue);
            command.Parameters.AddWithValue("@ResultUnit", record.ResultUnit);

            command.ExecuteNonQuery();
        }

        public List<QuantityHistoryRecord> GetAllRecords()
        {
            List<QuantityHistoryRecord> records = new List<QuantityHistoryRecord>();

            using NpgsqlConnection connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = "SELECT * FROM QuantityHistory ORDER BY Id DESC";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            using NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                records.Add(new QuantityHistoryRecord
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Category = reader["Category"].ToString() ?? string.Empty,
                    OperationType = reader["OperationType"].ToString() ?? string.Empty,
                    FirstValue = Convert.ToDouble(reader["FirstValue"]),
                    FirstUnit = reader["FirstUnit"].ToString() ?? string.Empty,
                    SecondValue = reader["SecondValue"] == DBNull.Value ? null : Convert.ToDouble(reader["SecondValue"]),
                    SecondUnit = reader["SecondUnit"] == DBNull.Value ? null : reader["SecondUnit"].ToString(),
                    TargetUnit = reader["TargetUnit"] == DBNull.Value ? null : reader["TargetUnit"].ToString(),
                    ResultValue = Convert.ToDouble(reader["ResultValue"]),
                    ResultUnit = reader["ResultUnit"].ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return records;
        }

        public QuantityHistoryRecord? GetRecordById(int id)
        {
            using NpgsqlConnection connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = "SELECT * FROM QuantityHistory WHERE Id = @Id";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new QuantityHistoryRecord
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Category = reader["Category"].ToString() ?? string.Empty,
                    OperationType = reader["OperationType"].ToString() ?? string.Empty,
                    FirstValue = Convert.ToDouble(reader["FirstValue"]),
                    FirstUnit = reader["FirstUnit"].ToString() ?? string.Empty,
                    SecondValue = reader["SecondValue"] == DBNull.Value ? null : Convert.ToDouble(reader["SecondValue"]),
                    SecondUnit = reader["SecondUnit"] == DBNull.Value ? null : reader["SecondUnit"].ToString(),
                    TargetUnit = reader["TargetUnit"] == DBNull.Value ? null : reader["TargetUnit"].ToString(),
                    ResultValue = Convert.ToDouble(reader["ResultValue"]),
                    ResultUnit = reader["ResultUnit"].ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }

            return null;
        }

        public bool DeleteRecord(int id)
        {
            using NpgsqlConnection connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = "DELETE FROM QuantityHistory WHERE Id = @Id";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
