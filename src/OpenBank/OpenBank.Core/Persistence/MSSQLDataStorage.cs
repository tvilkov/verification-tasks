using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using OpenBank.Core.Data;

namespace OpenBank.Core.Persistence
{
    public sealed class MSSQLDataStorage : IDataStorage
    {
        private readonly string m_ConnectionString;

        public MSSQLDataStorage(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            m_ConnectionString = connectionString;
            initializeStorage();
        }

        public void Save(TimedData data)
        {
            if (data == null) throw new ArgumentNullException("data");

            using (var connection = connect())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlQueries.SAVE_DATA;
                cmd.Parameters.AddWithValue("@timestamp", data.Timestamp);
                cmd.Parameters.AddWithValue("@value", data.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public ICollection<TimedData> LoadAll(DateTime @from, DateTime to)
        {
            using (var connection = connect())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlQueries.LOAD_DATA;
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@from", @from);
                using (var reader = cmd.ExecuteReader())
                {
                    Debug.Assert(reader != null);
                    var result = new Collection<TimedData>();
                    while (reader.Read())
                    {
                        var data = new TimedData(
                            reader.GetInt32(reader.GetOrdinal("Value")), 
                            reader.GetDateTime(reader.GetOrdinal("Timestamp")));
                        result.Add(data);
                    }
                    return result;
                }
            }
        }

        private void initializeStorage()
        {
             using (var connection = connect())
             using (var cmd = connection.CreateCommand())
             {
                 cmd.CommandType  = CommandType.Text;
                 cmd.CommandText = SqlQueries.INIT_STORAGE;
                 cmd.CommandTimeout = 60; // 1 min
                 cmd.ExecuteNonQuery();
             }
        }

        private SqlConnection connect()
        {
            var connection = new SqlConnection(m_ConnectionString);
            connection.Open();
            return connection;
        }

        abstract class SqlQueries
        {
            // TODO[tv]: Consider using stored procs instead of direct queries
            //          Consider moving string constants to resource files (one per query) for easier support
            public const string INIT_STORAGE = @"
IF OBJECT_ID('[dbo].[DataValues]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[DataValues](
	    [Timestamp] [datetime] NOT NULL,
	    [Value] [int] NOT NULL
    ) ON [PRIMARY];

    CREATE CLUSTERED INDEX [IX_DataValues_TimeStamp] ON [dbo].[DataValues] 
    (
	    [Timestamp] ASC
    ) WITH (
        PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = ON, 
        IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
        ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON
    ) ON [PRIMARY];
END";

            public const string SAVE_DATA = @"INSERT INTO DataValues([Timestamp], Value) VALUES(@timestamp, @value);";
            public const string LOAD_DATA = "SELECT [Timestamp], Value FROM DataValues" +
                                            "   WHERE [Timestamp] >= @from AND [Timestamp] <= @to" +
                                            "   ORDER BY [Timestamp]";
        }
    }
}