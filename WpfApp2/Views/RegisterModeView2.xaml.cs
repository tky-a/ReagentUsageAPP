using System.Windows;
using System.Windows.Input;
using WpfApp2.ViewModels;

namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : Window
    {
        private readonly RegisterModeView2ViewModel _viewModel;

        public RegisterModeView2()
        {
            InitializeComponent();
            _viewModel = new RegisterModeView2ViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// 入力ボックスでEnterキーが押された時の処理
        /// </summary>
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // ViewModelのNextCommandを実行
                if (_viewModel.NextCommand.CanExecute(null))
                {
                    _viewModel.NextCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// ウィンドウが読み込まれた時の処理
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初期フォーカスを入力ボックスに設定
            InputBox.Focus();
        }

        /// <summary>
        /// ウィンドウのキーダウン処理（グローバルなキーハンドリング）
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCキーで戻る処理
            if (e.Key == Key.Escape)
            {
                if (_viewModel.ReturnCommand.CanExecute(null))
                {
                    _viewModel.ReturnCommand.Execute(null);
                }
                e.Handled = true;
            }
            // F1キーで確認処理
            else if (e.Key == Key.F1)
            {
                if (_viewModel.ConfirmCommand.CanExecute(null))
                {
                    _viewModel.ConfirmCommand.Execute(null);
                }
                e.Handled = true;
            }
        }
    }
}