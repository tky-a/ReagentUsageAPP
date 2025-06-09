using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using WpfApp2.Models;
using WpfApp2.Helpers;
using System.Text.Json;

namespace WpfApp2.Models
{
    public class DatabaseManager
    {
        private readonly string _filePath;

        public DatabaseManager(string filePath = "reagents.json")
        {
            _filePath = filePath;
        }

        public async Task<List<ReagentModel>> LoadReagentsAsync()
        {
            // JSONファイルからReagentModelのリストを非同期で読み込む
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<ReagentModel>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var reagents = JsonSerializer.Deserialize<List<ReagentModel>>(json);
                return reagents ?? new List<ReagentModel>();
            }
            catch (Exception ex)
            {
                // ログ出力やエラーハンドリング
                throw new Exception($"データの読み込みに失敗しました: {ex.Message}");
            }
        }

        public async Task SaveReagentsAsync(List<ReagentModel> reagents)
        {
            // ReagentModelのリストをJSONファイルに非同期で保存する
            try
            {
                var json = JsonSerializer.Serialize(reagents, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                // ログ出力やエラーハンドリング
                throw new Exception($"データの保存に失敗しました: {ex.Message}");
            }
        }
    }
    public class ReagentModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string Class { get; set; } = "";
        public string UseStatus { get; set; } = "貸出可";
        public string Mass { get; set; } = "";
        public int LastUserID { get; set; }
        public string LastUseDate { get; set; } = "";
        public string FirstDate { get; set; } = "";
    }
}