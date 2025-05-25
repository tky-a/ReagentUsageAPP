using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace WpfApp2.Helper
{
    public class Reagent
    {
        public string 管理番号 { get; set; }
        public string 薬品名 { get; set; }
        public double 現在量 { get; set; }
        public double 容量 { get; set; }
        public string 登録日 { get; set; }
    }

    public static class DatabaseHelper
    {
        private static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reagent.db");
        private static string ConnectionString => $"Data Source={DbPath};";

        public static List<Reagent> SearchReagentsByName(string keyword)
        {
            var results = new List<Reagent>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Reagent WHERE 薬品名 LIKE @keyword";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new Reagent
                            {
                                管理番号 = reader["管理番号"].ToString(),
                                薬品名 = reader["薬品名"].ToString(),
                                現在量 = Convert.ToDouble(reader["現在量"]),
                                容量 = Convert.ToDouble(reader["容量"]),
                                登録日 = reader["登録日"].ToString()
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}
