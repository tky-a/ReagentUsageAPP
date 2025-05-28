using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Views;

namespace DrugManagerApp.Views
{
    /// <summary>
    /// FrontCoverView.xaml の相互作用ロジック
    /// </summary>
    public partial class FrontCoverView : Window
    {
        public FrontCoverView()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnToRegisterView_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new RegisterModeView();
            // 閉じた時に再表示する
            window.Closed += (s, args) =>
            {
                this.Show();
            };
            window.Show();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnToTESTView_Click(Object sender, RoutedEventArgs e)
        {
            this.Hide();
            var window = new RegisterModeView2();
            // 閉じた時に再表示する
            window.Closed += (s, args) =>
            {
                this.Show();
            };
            window.Show();
        }
    }
}
