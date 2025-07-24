using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using WpfApp2.Helpers;
using WpfApp2.Models;

namespace WpfApp2.Models
{
    public partial class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reagent.db");
            _connectionString = $"Data Source={dbPath};";
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public void EnsureTablesCreated()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                                    CREATE TABLE IF NOT EXISTS Users (
                                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserName TEXT NOT NULL
                                    );

                                    CREATE TABLE IF NOT EXISTS StorageLocations (
                                        LocationId INTEGER PRIMARY KEY AUTOINCREMENT,
                                        LocationName TEXT NOT NULL
                                    );

                                    CREATE TABLE IF NOT EXISTS Chemicals (
                                        ChemicalId INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT NOT NULL,
                                        Class TEXT,
                                        CurrentMass REAL,
                                        UseStatus TEXT,
                                        StorageLocationId INTEGER,
                                        LastUserId INTEGER,
                                        LastUseDate TEXT,
                                        FirstDate TEXT,
                                        FOREIGN KEY(StorageLocationId) REFERENCES StorageLocations(LocationId),
                                        FOREIGN KEY(LastUserId) REFERENCES Users(UserId)
                                    );

                                    CREATE TABLE IF NOT EXISTS UsageHistory (
                                        HistoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                                        ChemicalId INTEGER NOT NULL,
                                        UserId INTEGER NOT NULL,
                                        ActionType TEXT NOT NULL CHECK(ActionType IN('Withdraw', 'Return')),
                                        MassBefore REAL,
                                        MassAfter REAL,
                                        MassChange REAL,
                                        Notes TEXT,
                                        ActionDate TEXT NOT NULL,
                                        FOREIGN KEY(ChemicalId) REFERENCES Chemicals(ChemicalId),
                                        FOREIGN KEY(UserId) REFERENCES Users(UserId)
                                    );
                                ";
            command.ExecuteNonQuery();
        }

        public ObservableCollection<Chemical> GetAllChemicals()
        {
            var list = new ObservableCollection<Chemical>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var query = @"
                        SELECT c.ChemicalId, c.Name, c.Class, c.CurrentMass, c.UseStatus, 
                               c.StorageLocationId, sl.LocationName, c.LastUserId,
                               u.UserName as LastUserName, c.LastUseDate, c.FirstDate
                        FROM Chemicals c
                        LEFT JOIN StorageLocations sl ON c.StorageLocationId = sl.LocationId
                        LEFT JOIN Users u ON c.LastUserId = u.UserId";

            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Chemical
                {
                    ChemicalId = reader.GetInt32(reader.GetOrdinal("ChemicalId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Class = reader.GetString(reader.GetOrdinal("Class")),
                    CurrentMass = reader.GetDecimal(reader.GetOrdinal("CurrentMass")),
                    UseStatus = reader.GetString(reader.GetOrdinal("UseStatus")),
                    StorageLocationId = reader.GetInt32(reader.GetOrdinal("StorageLocationId")),
                    LocationName = reader.IsDBNull(reader.GetOrdinal("LocationName")) ? "" : reader.GetString(reader.GetOrdinal("LocationName")),
                    LastUserId = reader.IsDBNull(reader.GetOrdinal("LastUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("LastUserId")),
                    LastUserName = reader.IsDBNull(reader.GetOrdinal("LastUserName")) ? "" : reader.GetString(reader.GetOrdinal("LastUserName")),
                    LastUseDate = reader.IsDBNull(reader.GetOrdinal("LastUseDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastUseDate")),
                    FirstDate = reader.IsDBNull(reader.GetOrdinal("FirstDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FirstDate")),
                });
            }
            return list;
        }

        public ObservableCollection<User> GetAllUsers()
        {
            var list = new ObservableCollection<User>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = "SELECT UserId, UserName FROM Users";

            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new User
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return list;
        }

        public ObservableCollection<StorageLocation> GetAllStorageLocations()
        {
            var list = new ObservableCollection<StorageLocation>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = "SELECT LocationId, LocationName FROM StorageLocations";

            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new StorageLocation
                {
                    LocationId = reader.GetInt32(reader.GetOrdinal("LocationId")),
                    LocationName = reader.GetString(reader.GetOrdinal("LocationName"))
                });
            }
            return list;
        }

        public Chemical? GetChemicalById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Chemicals WHERE ChemicalId = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Chemical
                {
                    ChemicalId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Class = reader.GetString(2),
                    CurrentMass = reader.GetDecimal(3),
                    UseStatus = reader.GetString(4),
                    StorageLocationId = reader.GetInt32(5),
                    LastUserId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    LastUseDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    FirstDate = reader.GetDateTime(8)
                };
            }
            return null;
        }

        public User? GetUserNameById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE UserId = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    UserName = reader.GetString(1)

                };
            }
            return null;
        }

        public async Task<List<StorageLocation>> GetAllStorageLocationsAsync()
        {
            var list = new List<StorageLocation>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT LocationId, LocationName FROM StorageLocations";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new StorageLocation
                {
                    LocationId = reader.GetInt32(0),
                    LocationName = reader.GetString(1)
                });
            }

            return list;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var list = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT UserId, UserName FROM Users";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    UserName = reader.GetString(1)
                });
            }

            return list;
        }



        public void SaveUsageHistory(InputSet inputSet)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO UsageHistory
            (ChemicalId, UserId, ActionType, MassBefore, MassAfter, MassChange,Notes, ActionDate)
            VALUES
            (@ChemicalId, @UserId,@ActionType, @MassBefore, @MassAfter, @MassChange, @Notes, @ActionDate)";
            command.Parameters.AddWithValue("@ChemicalId", inputSet.InputReagentId);
            command.Parameters.AddWithValue("@UserId", inputSet.InputUserId);
            command.Parameters.AddWithValue("@ActionType", inputSet.ActionType ?? "");
            command.Parameters.AddWithValue("@MassBefore", inputSet.MassBefore ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MassAfter", inputSet.MassAfter ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MassChange", inputSet.MassChange ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Notes", inputSet.Notes ?? "");
            command.Parameters.AddWithValue("@ActionDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.ExecuteNonQuery();
        }

        public void UpdateChemicalAfterUsage(InputSet inputSet)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            string useStatus;
            decimal? newMass;

            if (inputSet.ActionType == "貸出")
            {
                useStatus = "貸出中";
                newMass = inputSet.MassBefore;
            }
            else // 返却
            {
                useStatus = "貸出可能";
                newMass = inputSet.MassAfter;
            }

            command.CommandText = @"
                                UPDATE Chemicals
                                SET CurrentMass = @CurrentMass,
                                    UseStatus = @UseStatus,
                                    LastUserId = @LastUserId,
                                    LastUseDate = @LastUseDate
                                WHERE ChemicalId = @ChemicalId";
            command.Parameters.AddWithValue("@CurrentMass", newMass ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UseStatus", useStatus);
            command.Parameters.AddWithValue("@LastUserId", inputSet.InputUserId);
            command.Parameters.AddWithValue("@LastUseDate", DateTime.Now);
            command.Parameters.AddWithValue("@ChemicalId", inputSet.InputReagentId);

            command.ExecuteNonQuery();
        }

        public async Task UpdateChemicalDataBase(Chemical chemical)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE Chemicals
            SET 
                Name = @Name,
                Class = @Class,
                CurrentMass = @CurrentMass,
                UseStatus = @UseStatus,
                StorageLocationId = @StorageLocationId,
                LastUserId = @LastUserId,
                LastUseDate = @LastUseDate,
                FirstDate = @FirstDate
            WHERE ChemicalId = @ChemicalId;
        ";

            command.Parameters.AddWithValue("@Name", chemical.Name ?? "");
            command.Parameters.AddWithValue("@Class", chemical.Class ?? "");
            command.Parameters.AddWithValue("@CurrentMass", chemical.CurrentMass);
            command.Parameters.AddWithValue("@UseStatus", chemical.UseStatus ?? "");
            command.Parameters.AddWithValue("@StorageLocationId", chemical.StorageLocationId);
            command.Parameters.AddWithValue("@LastUserId", chemical.LastUserId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LastUseDate", chemical.LastUseDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FirstDate", chemical.FirstDate.ToString("yyyy-MM-dd"));
            //command.Parameters.AddWithValue("@Note", chemical.Note ?? "");
            command.Parameters.AddWithValue("@ChemicalId", chemical.ChemicalId);

            await command.ExecuteNonQueryAsync();
        }

        public void ImportChemicalsFromCsv(string filePath)
        {
            using var parser = new TextFieldParser(filePath)
            {
                TextFieldType = FieldType.Delimited
            };
            parser.SetDelimiters(",");

            // ヘッダ処理
            string[] headers = parser.EndOfData ? Array.Empty<string>() : parser.ReadFields();
            if (headers.Length == 0) return;

            var headerToPropertyMap = new Dictionary<string, string>
            {
                { "薬品番号", "ChemicalId" },
                { "薬品名", "Name" },
                { "毒劇危", "Class" },
                { "使用状況", "UseStatus" },
                { "現在質量", "CurrentMass" },
                { "保管場所", "StorageLocationId" },
                { "使用者名", "LastUserId" },
                { "登録日", "FirstDate" },
                { "使用日", "LastUseDate" },
            };
           
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields.Length == 0) continue;

                var row = new Dictionary<string, string>();
                for (int i = 0; i < headers.Length && i < fields.Length; i++)
                {
                    row[headers[i]] = fields[i];
                }

                string GetValue(string column) => row.TryGetValue(column, out var value) ? value : string.Empty;

                string locationName = GetValue("保管場所");
                //string lastUserName = GetValue("使用者名");

                int storageLocationId = GetOrInsertStorageLocationId(locationName, connection);
                //int? lastUserId = string.IsNullOrWhiteSpace(lastUserName) ? null : GetOrInsertUserId(lastUserName, connection);

                var chemical = new Chemical
                {
                    ChemicalId = int.TryParse(GetValue("薬品番号"), out var id) ? id : 0,
                    Name = GetValue("薬品名"),
                    Class = GetValue("毒劇危"),
                    UseStatus = GetValue("使用状況"),
                    CurrentMass = decimal.TryParse(GetValue("現在質量"), out var mass) ? mass : 0,
                    StorageLocationId = storageLocationId,
                    LocationName = locationName,
                    //LastUserId = lastUserId,
                    //LastUserName = lastUserName,
                    LastUseDate = DateTime.TryParse(GetValue("最終使用日"), out var lastUseDate) ? lastUseDate : null,
                    FirstDate = DateTime.TryParse(GetValue("登録日"), out var firstDate) ? firstDate : DateTime.Now,
                };

                AddChemical(chemical);
            }
        }

        public void AddChemical(Chemical chemical)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"
                        INSERT OR REPLACE INTO Chemicals (
                            ChemicalId, Name, Class, UseStatus, CurrentMass,
                            StorageLocationId, LastUserId,
                            LastUseDate, FirstDate
                        )
                        VALUES (
                            @chemicalId, @name, @class, @useStatus, @currentMass,
                            @storageLocationId,  @lastUserId,
                            @lastUseDate, @firstDate
                        );";

            using var command = new SqliteCommand(query, connection);

            command.Parameters.AddWithValue("@chemicalId", chemical.ChemicalId);
            command.Parameters.AddWithValue("@name", chemical.Name ?? "");
            command.Parameters.AddWithValue("@class", chemical.Class ?? "");
            command.Parameters.AddWithValue("@currentMass", chemical.CurrentMass);
            command.Parameters.AddWithValue("@useStatus", chemical.UseStatus = null ?? "貸出可能");
            command.Parameters.AddWithValue("@storageLocationId", chemical.StorageLocationId);
            command.Parameters.AddWithValue("@lastUserId", (object?)chemical.LastUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@lastUseDate", (object?)chemical.LastUseDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@firstDate", chemical.FirstDate);

            command.ExecuteNonQuery();
        }

        private int GetOrInsertStorageLocationId(string locationName, SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT LocationId FROM StorageLocations WHERE LocationName = @locationName;";
            cmd.Parameters.AddWithValue("@locationName", locationName);
            var result = cmd.ExecuteScalar();
            if (result != null) return Convert.ToInt32(result);

            using var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO StorageLocations (LocationName) VALUES (@locationName);";
            insertCmd.Parameters.AddWithValue("@locationName", locationName);
            insertCmd.ExecuteNonQuery();
            //return (int)connection.LastInsertRowId;
            using var getIdCmd = connection.CreateCommand();
            getIdCmd.CommandText = "SELECT last_insert_rowid();";
            return Convert.ToInt32(getIdCmd.ExecuteScalar() ?? 0);
        }

        private int GetOrInsertUserId(string userName, SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId FROM Users WHERE UserName = @userName;";
            cmd.Parameters.AddWithValue("@name", userName);
            var result = cmd.ExecuteScalar();
            if (result != null) return Convert.ToInt32(result);

            using var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Users (UserName) VALUES (@userName);";
            insertCmd.Parameters.AddWithValue("@userName", userName);
            insertCmd.ExecuteNonQuery();
            //return (int)connection.LastInsertRowId;
            using var getIdCmd = connection.CreateCommand();
            getIdCmd.CommandText = "SELECT last_insert_rowid();";
            return Convert.ToInt32(getIdCmd.ExecuteScalar() ?? 0);
        }


    }
}