using System;
using System.Text.RegularExpressions;

namespace WpfApp2.Helpers
{
    public static class ValidationHelper
    {
        // 管理番号の検証
        public static bool IsValidItemId(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return false;

            // 管理番号は英数字で3-20文字
            var pattern = @"^[A-Za-z0-9]{3,20}$";
            return Regex.IsMatch(itemId, pattern);
        }

        // ユーザーIDの検証
        public static bool IsValidUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            // ユーザーIDは英数字で3-20文字
            var pattern = @"^[A-Za-z0-9]{3,20}$";
            return Regex.IsMatch(userId, pattern);
        }

        // 質量の検証
        public static bool IsValidWeight(string weight)
        {
            if (string.IsNullOrWhiteSpace(weight))
                return false;

            // 正の数値（小数点可）
            if (double.TryParse(weight, out double result))
            {
                return result >= 0 && result <= 999999.99;
            }

            return false;
        }

        // 薬品名の検証
        public static bool IsValidDrugName(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
                return false;

            // 薬品名は1-100文字
            return drugName.Length >= 1 && drugName.Length <= 100;
        }

        // 容量の検証
        public static bool IsValidCapacity(string capacity)
        {
            if (string.IsNullOrWhiteSpace(capacity))
                return false;

            // 正の数値（小数点可）
            if (double.TryParse(capacity, out double result))
            {
                return result > 0 && result <= 999999.99;
            }

            return false;
        }

        // メールアドレスの検証
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        // 電話番号の検証
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // 日本の電話番号形式（ハイフンありなし両対応）
            var pattern = @"^(0\d{1,4}-?\d{1,4}-?\d{4}|0\d{9,10})$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        // パスワードの検証
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // パスワードは8文字以上、英数字を含む
            if (password.Length < 8)
                return false;

            bool hasLetter = Regex.IsMatch(password, @"[a-zA-Z]");
            bool hasNumber = Regex.IsMatch(password, @"\d");

            return hasLetter && hasNumber;
        }

        // 日付の検証
        public static bool IsValidDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return false;

            return DateTime.TryParse(dateString, out DateTime result) &&
                   result >= new DateTime(1900, 1, 1) &&
                   result <= DateTime.Now.AddYears(10);
        }

        // 数値範囲の検証
        public static bool IsInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        // 文字列長の検証
        public static bool IsValidLength(string text, int minLength, int maxLength)
        {
            if (text == null)
                return minLength == 0;

            return text.Length >= minLength && text.Length <= maxLength;
        }

        // 必須項目の検証
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        // バーコードの検証（簡易版）
        public static bool IsValidBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            // バーコードは数字のみで8桁以上
            var pattern = @"^\d{8,}$";
            return Regex.IsMatch(barcode, pattern);
        }

        // 単位の検証
        public static bool IsValidUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return false;

            // 許可される単位
            var validUnits = new[] { "g", "kg", "ml", "L", "mg", "μg", "mol", "mmol" };
            return Array.Exists(validUnits, u => u.Equals(unit, StringComparison.OrdinalIgnoreCase));
        }

        // 特殊文字の検証（SQLインジェクション対策）
        public static bool ContainsDangerousCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // 危険な文字列をチェック
            var dangerousPatterns = new[]
            {
                @"[';--]",
                @"(union|select|insert|update|delete|drop|create|alter|exec|execute)",
                @"<script",
                @"javascript:",
                @"vbscript:"
            };

            foreach (var pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // 複合検証：薬品データ全体の検証
        public static ValidationResult ValidateDrugItem(string itemId, string drugName, string capacity, string currentWeight, string unit)
        {
            var result = new ValidationResult();

            if (!IsValidItemId(itemId))
            {
                result.AddError("管理番号", "管理番号は英数字3-20文字で入力してください。");
            }

            if (!IsValidDrugName(drugName))
            {
                result.AddError("薬品名", "薬品名は1-100文字で入力してください。");
            }

            if (!IsValidCapacity(capacity))
            {
                result.AddError("容量", "容量は正の数値で入力してください。");
            }

            if (!IsValidWeight(currentWeight))
            {
                result.AddError("現在量", "現在量は正の数値で入力してください。");
            }

            if (!IsValidUnit(unit))
            {
                result.AddError("単位", "単位は g, kg, ml, L, mg, μg, mol, mmol のいずれかを入力してください。");
            }

            // 現在量が容量を超えていないかチェック
            if (result.IsValid && IsValidCapacity(capacity) && IsValidWeight(currentWeight))
            {
                double cap = double.Parse(capacity);
                double weight = double.Parse(currentWeight);
                if (weight > cap)
                {
                    result.AddError("現在量", "現在量は容量を超えることはできません。");
                }
            }

            return result;
        }
    }

    // バリデーション結果を格納するクラス
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

        public void AddError(string field, string message)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new List<string>();
            }
            Errors[field].Add(message);
        }

        public string GetErrorsForField(string field)
        {
            if (Errors.ContainsKey(field))
            {
                return string.Join("\n", Errors[field]);
            }
            return string.Empty;
        }

        public string GetAllErrors()
        {
            var allErrors = new List<string>();
            foreach (var fieldErrors in Errors.Values)
            {
                allErrors.AddRange(fieldErrors);
            }
            return string.Join("\n", allErrors);
        }
    }
}