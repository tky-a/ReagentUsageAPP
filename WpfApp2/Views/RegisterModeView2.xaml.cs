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
            HedderUserEllipse.Loaded += (_, __) => UpdatelinePositions();
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

        private void HedderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatelinePositions();
        }

        private void UpdatelinePositions()
        {
            if(!HedderReagentIDEllipse.IsLoaded 
                || !HedderMassEllipse.IsLoaded
                || !HedderUserEllipse.IsLoaded)
            {
                return; // Ellipseがまだロードされていない場合は何もしない
            }

            Point center1 = HedderReagentIDEllipse.TranslatePoint(
                new Point(HedderReagentIDEllipse.ActualWidth / 2, HedderReagentIDEllipse.ActualHeight / 2), lineCanvas);

            Point center2 = HedderMassEllipse.TranslatePoint(
                new Point(HedderMassEllipse.ActualWidth / 2, HedderMassEllipse.ActualHeight / 2), lineCanvas);

            Point center3 = HedderUserEllipse.TranslatePoint(
                new Point(HedderUserEllipse.ActualWidth / 2, HedderUserEllipse.ActualHeight / 2), lineCanvas);

            if (firstLine != null && secondLine != null)
            {
                firstLine.X1 = center1.X;
                firstLine.Y1 = center1.Y;
                firstLine.X2 = center2.X;
                firstLine.Y2 = center2.Y;

                secondLine.X1 = center2.X;
                secondLine.Y1 = center2.Y;
                secondLine.X2 = center3.X;
                secondLine.Y2 = center3.Y;
            }


        }
    }
}