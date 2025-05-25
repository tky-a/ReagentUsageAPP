using System;
using System.Windows;
using WpfApp2.Views;
using WpfApp2.Services;

namespace WpfApp2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 未処理例外のハンドリング
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // アプリケーションの初期化
                InitializeApplication();

                // メインウィンドウを表示
                var mainWindow = new RegisterModeView();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"アプリケーションの起動に失敗しました: {ex.Message}",
                    "起動エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        private void InitializeApplication()
        {
            // 設定の読み込み
            LoadApplicationSettings();

            // サービスの初期化
            InitializeServices();
        }

        private void LoadApplicationSettings()
        {
            // アプリケーション設定の読み込み
            // 実際の運用では設定ファイルから読み込み
        }

        private void InitializeServices()
        {
            // 認証サービスの初期化
            var authService = AuthenticationService.Instance;

            // その他のサービスの初期化が必要な場合はここで行う
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show($"予期しないエラーが発生しました:\n{e.Exception.Message}",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);

                // ログ出力（実際の運用では適切なログシステムを使用）
                LogError(e.Exception);

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"致命的なエラーが発生しました:\n{ex.Message}",
                    "致命的エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                MessageBox.Show($"致命的なエラーが発生しました:\n{exception?.Message}",
                    "致命的エラー", MessageBoxButton.OK, MessageBoxImage.Error);

                LogError(exception);
            }
            catch
            {
                // 最後の手段
                MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。",
                    "致命的エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogError(Exception exception)
        {
            try
            {
                // 簡易ログ出力（実際の運用では適切なログシステムを使用）
                string logPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DrugLendingSystem","DrugLendingSystem","error.log");

                // ログディレクトリが存在しない場合は作成
                var logDirectory = System.IO.Path.GetDirectoryName(logPath);
                if (!System.IO.Directory.Exists(logDirectory))
                {
                    System.IO.Directory.CreateDirectory(logDirectory);
                }

                // エラー情報をログファイルに追記
                using (var writer = new System.IO.StreamWriter(logPath, true))
                {
                    writer.WriteLine("----- エラー発生日時: {0} -----", DateTime.Now);
                    writer.WriteLine("メッセージ: {0}", exception?.Message);
                    writer.WriteLine("スタックトレース: {0}", exception?.StackTrace);
                    writer.WriteLine();
                }
            }
            catch
            {
                // ログ出力自体に失敗した場合は何もしない（安全に無視）
            }
        }
    }
}
