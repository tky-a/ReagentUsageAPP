using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Models;

namespace WpfApp2.Views
{
    /// <summary>
    /// ManagerModeView.xaml の相互作用ロジック
    /// </summary>
    public partial class EditInputSetWindow : Window
    {
        public EditInputSetWindow(InputSet inputSet)
        {
            InitializeComponent();
            DataContext = inputSet;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
