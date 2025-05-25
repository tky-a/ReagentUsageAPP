using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Helpers;

namespace WpfApp2.Models
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public DatabaseManager()
        {
            _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DrugLendingSystem", "database.db");
            _connectionString = $"Data Source={_databasePath}";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                // ディレクトリが存在しない場合は作成
                string directory = Path.GetDirectoryName(_databasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    CreateTables(connection);
                    InsertSampleData(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データベースの初期化に失敗しました: {ex.Message}");
            }
        }

        private void CreateTables(SqliteConnection connection)
        {
            string createDrugItemsTable = @"
                CREATE TABLE IF NOT EXISTS DrugItems (
                    ItemId TEXT PRIMARY KEY,
                    DrugName TEXT NOT NULL,
                    TotalCapacity REAL NOT NULL,
                    CurrentWeight REAL NOT NULL,
                    Unit TEXT NOT NULL DEFAULT 'g',
                    RegistrationDate TEXT NOT NULL,
                    LendingDate TEXT,
                    ReturnDate TEXT,
                    UserId TEXT,
                    IsLent INTEGER NOT NULL DEFAULT 0,
                    Notes TEXT,
                    Location TEXT,
                    Category TEXT
                );";

            string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId TEXT PRIMARY KEY,
                    UserName TEXT NOT NULL,
                    Department TEXT,
                    Email TEXT,
                    RegistrationDate TEXT NOT NULL
                );";

            string createLendingHistoryTable = @"
                CREATE TABLE IF NOT EXISTS LendingHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ItemId TEXT NOT NULL,
                    UserId TEXT NOT NULL,
                    LendingDate TEXT NOT NULL,
                    ReturnDate TEXT,
                    LendingWeight REAL,
                    ReturnWeight REAL,
                    Notes TEXT,
                    FOREIGN KEY (ItemId) REFERENCES DrugItems (ItemId),
                    FOREIGN KEY (UserId) REFERENCES Users (UserId)
                );";

            using (var command = new SqliteCommand(createDrugItemsTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand(createUsersTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand(createLendingHistoryTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertSampleData(SqliteConnection connection)
        {
            // サンプルデータが既に存在するかチェック
            string checkQuery = "SELECT COUNT(*) FROM DrugItems";
            using (var command = new SqliteCommand(checkQuery, connection))
            {
                long count = (long)command.ExecuteScalar();
                if (count > 0) return; // 既にデータが存在する場合はスキップ
            }

            // サンプル薬品データを挿入
            var sampleDrugs = new[]
            {
                new { ItemId = "D001", DrugName = "エタノール", TotalCapacity = 500.0, CurrentWeight = 450.0, Unit = "ml", Location = "棚A-1", Category = "溶媒" },
                new { ItemId = "D002", DrugName = "塩酸", TotalCapacity = 100.0, CurrentWeight = 85.0, Unit = "ml", Location = "棚A-2", Category = "酸" },
                new { ItemId = "D003", DrugName = "水酸化ナトリウム", TotalCapacity = 250.0, CurrentWeight = 200.0, Unit = "g", Location = "棚B-1", Category = "塩基" },
                new { ItemId = "D004", DrugName = "アセトン", TotalCapacity = 1000.0, CurrentWeight = 750.0, Unit = "ml", Location = "棚C-1", Category = "溶媒" },
                new { ItemId = "D005", DrugName = "硫酸", TotalCapacity = 500.0, CurrentWeight = 300.0, Unit = "ml", Location = "棚A-3", Category = "酸" }
            };

            foreach (var drug in sampleDrugs)
            {
                string insertQuery = @"
                    INSERT INTO DrugItems (ItemId, DrugName, TotalCapacity, CurrentWeight, Unit, 
                                         RegistrationDate, IsLent, Location, Category)
                    VALUES (@ItemId, @DrugName, @TotalCapacity, @CurrentWeight, @Unit, 
                           @RegistrationDate, 0, @Location, @Category)";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ItemId", drug.ItemId);
                    command.Parameters.AddWithValue("@DrugName", drug.DrugName);
                    command.Parameters.AddWithValue("@TotalCapacity", drug.TotalCapacity);
                    command.Parameters.AddWithValue("@CurrentWeight", drug.CurrentWeight);
                    command.Parameters.AddWithValue("@Unit", drug.Unit);
                    command.Parameters.AddWithValue("@RegistrationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@Location", drug.Location);
                    command.Parameters.AddWithValue("@Category", drug.Category);
                    command.ExecuteNonQuery();
                }
            }

            // サンプルユーザーデータを挿入
            var sampleUsers = new[]
            {
                new { UserId = "U001", UserName = "田中太郎", Department = "化学科", Email = "tanaka@example.com" },
                new { UserId = "U002", UserName = "佐藤花子", Department = "生物学科", Email = "sato@example.com" },
                new { UserId = "U003", UserName = "山田次郎", Department = "物理学科", Email = "yamada@example.com" }
            };

            foreach (var user in sampleUsers)
            {
                string insertQuery = @"
                    INSERT INTO Users (UserId, UserName, Department, Email, RegistrationDate)
                    VALUES (@UserId, @UserName, @Department, @Email, @RegistrationDate)";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Department", user.Department);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@RegistrationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<DrugItem> GetAllDrugItems()
        {
            var drugItems = new List<DrugItem>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM DrugItems ORDER BY DrugName";

                    using (var command = new SqliteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            drugItems.Add(MapReaderToDrugItem(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品データの取得に失敗しました: {ex.Message}");
            }

            return drugItems;
        }

        public DrugItem GetDrugItemById(string itemId)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM DrugItems WHERE ItemId = @ItemId";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToDrugItem(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品データの取得に失敗しました: {ex.Message}");
            }

            return null;
        }

        public List<DrugItem> SearchDrugItems(string searchQuery)
        {
            var drugItems = new List<DrugItem>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT * FROM DrugItems 
                        WHERE DrugName LIKE @SearchQuery 
                           OR ItemId LIKE @SearchQuery 
                           OR Category LIKE @SearchQuery 
                           OR Location LIKE @SearchQuery
                        ORDER BY DrugName";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                drugItems.Add(MapReaderToDrugItem(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品検索に失敗しました: {ex.Message}");
            }

            return drugItems;
        }

        public void UpdateDrugItem(DrugItem drugItem)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE DrugItems SET 
                            DrugName = @DrugName,
                            TotalCapacity = @TotalCapacity,
                            CurrentWeight = @CurrentWeight,
                            Unit = @Unit,
                            LendingDate = @LendingDate,
                            ReturnDate = @ReturnDate,
                            UserId = @UserId,
                            IsLent = @IsLent,
                            Notes = @Notes,
                            Location = @Location,
                            Category = @Category
                        WHERE ItemId = @ItemId";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", drugItem.管理番号);
                        command.Parameters.AddWithValue("@DrugName", drugItem.薬品名);
                        command.Parameters.AddWithValue("@TotalCapacity", drugItem.TotalCapacity);
                        command.Parameters.AddWithValue("@CurrentWeight", drugItem.CurrentWeight);
                        command.Parameters.AddWithValue("@Unit", drugItem.Unit);
                        command.Parameters.AddWithValue("@LendingDate", drugItem.LendingDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@ReturnDate", drugItem.ReturnDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@UserId", drugItem.UserId ?? "");
                        command.Parameters.AddWithValue("@IsLent", drugItem.IsLent ? 1 : 0);
                        command.Parameters.AddWithValue("@Notes", drugItem.Notes ?? "");
                        command.Parameters.AddWithValue("@Location", drugItem.Location ?? "");
                        command.Parameters.AddWithValue("@Category", drugItem.Category ?? "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品データの更新に失敗しました: {ex.Message}");
            }
        }

        public void InsertDrugItem(DrugItem drugItem)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO DrugItems (ItemId, DrugName, TotalCapacity, CurrentWeight, Unit, 
                                             RegistrationDate, LendingDate, ReturnDate, UserId, IsLent, 
                                             Notes, Location, Category)
                        VALUES (@ItemId, @DrugName, @TotalCapacity, @CurrentWeight, @Unit, 
                               @RegistrationDate, @LendingDate, @ReturnDate, @UserId, @IsLent, 
                               @Notes, @Location, @Category)";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", drugItem.管理番号);
                        command.Parameters.AddWithValue("@DrugName", drugItem.薬品名);
                        command.Parameters.AddWithValue("@TotalCapacity", drugItem.TotalCapacity);
                        command.Parameters.AddWithValue("@CurrentWeight", drugItem.CurrentWeight);
                        command.Parameters.AddWithValue("@Unit", drugItem.Unit);
                        command.Parameters.AddWithValue("@RegistrationDate", drugItem.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@LendingDate", drugItem.LendingDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@ReturnDate", drugItem.ReturnDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@UserId", drugItem.UserId ?? "");
                        command.Parameters.AddWithValue("@IsLent", drugItem.IsLent ? 1 : 0);
                        command.Parameters.AddWithValue("@Notes", drugItem.Notes ?? "");
                        command.Parameters.AddWithValue("@Location", drugItem.Location ?? "");
                        command.Parameters.AddWithValue("@Category", drugItem.Category ?? "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品データの挿入に失敗しました: {ex.Message}");
            }
        }

        public void DeleteDrugItem(string itemId)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM DrugItems WHERE ItemId = @ItemId";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"薬品データの削除に失敗しました: {ex.Message}");
            }
        }

        public void AddLendingHistory(string itemId, string userId, DateTime lendingDate, double lendingWeight)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO LendingHistory (ItemId, UserId, LendingDate, LendingWeight)
                        VALUES (@ItemId, @UserId, @LendingDate, @LendingWeight)";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@LendingDate", lendingDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@LendingWeight", lendingWeight);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"貸出履歴の追加に失敗しました: {ex.Message}");
            }
        }

        private DrugItem MapReaderToDrugItem(SqliteDataReader reader)
        {
            var drugItem = new DrugItem
            {
                管理番号 = reader["ItemId"].ToString(),
                薬品名 = reader["DrugName"].ToString(),
                TotalCapacity = Convert.ToDouble(reader["TotalCapacity"]),
                CurrentWeight = Convert.ToDouble(reader["CurrentWeight"]),
                Unit = reader["Unit"].ToString(),
                RegistrationDate = DateTime.Parse(reader["RegistrationDate"].ToString()),
                UserId = reader["UserId"].ToString(),
                IsLent = Convert.ToBoolean(reader["IsLent"]),
                Notes = reader["Notes"].ToString(),
                Location = reader["Location"].ToString(),
                Category = reader["Category"].ToString()
            };

            // Nullable な日付フィールドの処理
            if (reader["LendingDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["LendingDate"].ToString()))
            {
                drugItem.LendingDate = DateTime.Parse(reader["LendingDate"].ToString());
            }

            if (reader["ReturnDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["ReturnDate"].ToString()))
            {
                drugItem.ReturnDate = DateTime.Parse(reader["ReturnDate"].ToString());
            }

            return drugItem;
        }
    }
}