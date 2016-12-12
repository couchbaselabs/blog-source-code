using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Dapper;

namespace ConsoleApplication
{
    public class Program
    {
        static IDbConnection _db;
        static IBucket _bucket;

        static void InitializeDbConnections()
        {
            _db = new SqlConnection("Data Source=(local);Trusted_Connection=true;Initial Catalog=NORTHWND;");
            _db.Open();

            ClusterHelper.Initialize(new ClientConfiguration {
                Servers = new List<Uri> { new Uri("couchbase://localhost")}
            });
            _bucket = ClusterHelper.GetBucket("Northwind");
        }

        static void ProcessRow(dynamic row, string tableName, dynamic primaryKeys)
        {
            var doc = new Document<dynamic> {
                Id = GenerateKey(tableName, row, primaryKeys),
                Content = GenerateDocument(row, tableName)
            };
            _bucket.Insert(doc);
            Console.Write(".");            
        }
        
        static string GenerateKey(string tableName, dynamic row, dynamic primaryKeys)
        {
            var key = tableName;
            var rowdict = (IDictionary<string, object>)row;
            foreach(var primaryKey in primaryKeys) {
                key += "::" + rowdict[primaryKey.ColumnName];
            }
            return key;
        }

        static dynamic GenerateDocument(dynamic row, string tableName)
        {
            var doc = row;
            doc.Type = tableName;
            return doc;
        }

        static void ProcessTable(string tableName)
        {
            // get all column names
            var columnNamesSql = @"
                SELECT COLUMN_NAME AS [Name], DATA_TYPE AS DataType
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @Name";
            var columns = _db.Query<dynamic>(columnNamesSql, new { tableName }).ToList();

            // get all primary key column names
            var primaryKeyColumnsSql = @"
                SELECT k.COLUMN_NAME AS [ColumnName]
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS c
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE k ON c.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                WHERE c.CONSTRAINT_TYPE = 'PRIMARY KEY'
                AND c.TABLE_NAME = @Name";
            var primaryKeys = _db.Query<dynamic>(primaryKeyColumnsSql, new { tableName }).ToList();

            // get all data from the table
            var dataSql = $"SELECT * FROM [{tableName}]";
            var data = _db.Query<dynamic>(dataSql).ToList();

            foreach(var row in data) {
                ProcessRow(row, tableName, primaryKeys);
            }            
        }

        public static void Main(string[] args)
        {
            InitializeDbConnections();

            // get all the tables to migrate
            var tables = _db.Query<dynamic>(@"
                SELECT TABLE_NAME AS [Name]
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
                AND TABLE_NAME <> 'sysdiagrams'");

            // migrate each row from each table into a document
            foreach(var table in tables)
            {
                Console.WriteLine(table.Name);

                ProcessTable(table.Name);

                Console.WriteLine();                
            }

            // cleanup
            _db.Close();
            ClusterHelper.Close();
        }
    }
}
