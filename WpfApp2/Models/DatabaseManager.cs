using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Helpers;
using System.Text.Json;
using System.Collections.ObjectModel;

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
    }
}