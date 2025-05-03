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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Helper;

namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView : Window
    {
        private bool _isSearchOpen = true;

        public RegisterModeView()
        {
            InitializeComponent();
        }
        public class ScanResult
        {
            public string ItemId { get; set; }
            public double Weight { get; set; }
            public string UserId { get; set; }
        }
        private void OnEditFinishButtonClick(object sender, RoutedEventArgs e)
        {
            ResultItemIdBox.IsReadOnly = true;
            ResultWeightBox.IsReadOnly = true;
            ResultUserIdBox.IsReadOnly = true;
        }
        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                switch (clickedButton.Name)
                {
                    case "ResultItemBTN":
                        ResultItemIdBox.IsReadOnly = false;
                        break;
                    case "ResultWeightBTN":
                        ResultWeightBox.IsReadOnly = false;
                        break;
                    case "ResultUserIdBTN":
                        ResultUserIdBox.IsReadOnly = false;
                        break;
                    default:
                        MessageBox.Show("Unknown button clicked");
                        break;
                }
            }
        }

        private void ToggleSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var parentGrid = (Grid)this.Content;
            var searchColumn = parentGrid.ColumnDefinitions[3]; // 4ペイン目

            if (_isSearchOpen)
            {
                // 閉じる（幅を0に）
                searchColumn.Width = new GridLength(20);
                ToggleSearchButton.Content = "＜";
                _isSearchOpen = false;
            }
            else
            {
                // 開く（幅を固定値に戻す）
                searchColumn.Width = new GridLength(200); // 好きな固定幅に
                ToggleSearchButton.Content = "＞";
                _isSearchOpen = true;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("検索キーワードを入力してください。", "注意", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<Reagent> results = DatabaseHelper.SearchReagentsByName(keyword);
            SearchResultsListView.ItemsSource = results;
        }
    }
}
