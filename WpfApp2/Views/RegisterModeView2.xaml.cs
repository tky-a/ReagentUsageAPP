using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using WpfApp2.ViewModels;

namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : UserControl
    {

        private readonly RegisterModeView2ViewModel _viewModel;

        public RegisterModeView2()
        {
            InitializeComponent();
            this.Loaded += (s,e) =>InputBox.Focus();            
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
                    InputBoxFocus(sender, null);
                }
                e.Handled = true;
            }
        }
        private void InputBoxFocus(object sender, RoutedEventArgs? e)
        {
            InputBox.Focus();
        }
    }
}