using System;
using System.Security.Cryptography;
using System.Text;

namespace WpfApp2.Services
{
    public class AuthenticationService
    {
        private static AuthenticationService _instance;
        private static readonly object _lock = new object();

        public static AuthenticationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new AuthenticationService();
                    }
                }
                return _instance;
            }
        }

        private string _currentUserId;
        private string _currentUserName;
        private UserRole _currentUserRole;
        private DateTime _loginTime;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUserId);
        public string CurrentUserId => _currentUserId;
        public string CurrentUserName => _currentUserName;
        public UserRole CurrentUserRole => _currentUserRole;
        public DateTime LoginTime => _loginTime;

        private AuthenticationService()
        {
            _currentUserId = "";
            _currentUserName = "";
            _currentUserRole = UserRole.None;
        }

        // ログイン処理
        public AuthenticationResult Login(string userId, string password)
        {
            try
            {
                // 簡易認証（実際の運用では適切な認証システムを使用）
                if (ValidateCredentials(userId, password))
                {
                    _currentUserId = userId;
                    _currentUserName = GetUserName(userId);
                    _currentUserRole = GetUserRole(userId);
                    _loginTime = DateTime.Now;

                    return new AuthenticationResult
                    {
                        IsSuccess = true,
                        Message = "ログインに成功しました。",
                        UserId = _currentUserId,
                        UserName = _currentUserName,
                        UserRole = _currentUserRole
                    };
                }
                else
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        Message = "ユーザーIDまたはパスワードが正しくありません。"
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Message = $"ログイン処理でエラーが発生しました: {ex.Message}"
                };
            }
        }

        // ログアウト処理
        public void Logout()
        {
            _currentUserId = "";
            _currentUserName = "";
            _currentUserRole = UserRole.None;
            _loginTime = default(DateTime);
        }

        // 権限チェック
        public bool HasPermission(Permission permission)
        {
            if (!IsAuthenticated)
                return false;

            switch (_currentUserRole)
            {
                case UserRole.Administrator:
                    return true; // 管理者は全権限
                case UserRole.Manager:
                    return permission != Permission.SystemSettings;
                case UserRole.User:
                    return permission == Permission.BorrowItems || permission == Permission.ReturnItems;
                default:
                    return false;
            }
        }

        // パスワードのハッシュ化
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "DrugLendingSystemSalt"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // 認証情報の検証（簡易版）
        private bool ValidateCredentials(string userId, string password)
        {
            // 実際の運用では、データベースから暗号化されたパスワードを取得して比較
            // ここでは簡易的な認証を実装
            var defaultUsers = new Dictionary<string, (string password, UserRole role, string name)>
            {
                { "admin", ("admin123", UserRole.Administrator, "管理者") },
                { "manager", ("manager123", UserRole.Manager, "管理者") },
                { "user1", ("user123", UserRole.User, "一般ユーザー1") },
                { "user2", ("user123", UserRole.User, "一般ユーザー2") }
            };

            if (defaultUsers.ContainsKey(userId.ToLower()))
            {
                var userInfo = defaultUsers[userId.ToLower()];
                return userInfo.password == password;
            }

            return false;
        }

        private string GetUserName(string userId)
        {
            var defaultUsers = new Dictionary<string, string>
            {
                { "admin", "システム管理者" },
                { "manager", "マネージャー" },
                { "user1", "田中太郎" },
                { "user2", "佐藤花子" }
            };

            return defaultUsers.ContainsKey(userId.ToLower())
                ? defaultUsers[userId.ToLower()]
                : userId;
        }

        private UserRole GetUserRole(string userId)
        {
            var defaultUsers = new Dictionary<string, UserRole>
            {
                { "admin", UserRole.Administrator },
                { "manager", UserRole.Manager },
                { "user1", UserRole.User },
                { "user2", UserRole.User }
            };

            return defaultUsers.ContainsKey(userId.ToLower())
                ? defaultUsers[userId.ToLower()]
                : UserRole.User;
        }

        // セッション有効性チェック
        public bool IsSessionValid()
        {
            if (!IsAuthenticated)
                return false;

            // セッションタイムアウトは8時間
            TimeSpan sessionDuration = DateTime.Now - _loginTime;
            return sessionDuration.TotalHours < 8;
        }

        // パスワード強度チェック
        public PasswordStrength CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return PasswordStrength.VeryWeak;

            int score = 0;

            // 長さチェック
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // 文字種チェック
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"\d")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?\"":{}|<>]")) score++;

            return score switch
            {
                0 or 1 => PasswordStrength.VeryWeak,
                2 => PasswordStrength.Weak,
                3 or 4 => PasswordStrength.Medium,
                5 => PasswordStrength.Strong,
                _ => PasswordStrength.VeryStrong
            };
        }
    }

    // 認証結果を格納するクラス
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public UserRole UserRole { get; set; }
    }

    // ユーザーロール
    public enum UserRole
    {
        None,
        User,        // 一般ユーザー
        Manager,     // 管理者
        Administrator // システム管理者
    }

    // 権限
    public enum Permission
    {
        BorrowItems,      // 薬品借用
        ReturnItems,      // 薬品返却
        ViewInventory,    // 在庫閲覧
        ManageInventory,  // 在庫管理
        ViewReports,      // レポート閲覧
        ManageUsers,      // ユーザー管理
        SystemSettings    // システム設定
    }

    // パスワード強度
    public enum PasswordStrength
    {
        VeryWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }
}