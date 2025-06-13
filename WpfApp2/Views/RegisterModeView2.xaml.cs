using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text;
using WpfApp2.Models;


namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : Window
    {
        private readonly DatabaseManager _databaseManager;
        private ObservableCollection<Chemical> _chemicals;
        private ObservableCollection<User> _users;
        private ObservableCollection<StorageLocation> _storageLocations;

        public RegisterModeView2()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
            _databaseManager.EnsureTablesCreated();
            DataContext = this;
            FirstViewPanel.Visibility = Visibility.Visible;
            RegisterModePanel.Visibility = Visibility.Hidden;
            btnNext.IsEnabled = true;
            btnReturn.Visibility = Visibility.Hidden; // 初期状態では戻るボタンを非表示
        }

        private void SetupUI()
        {
            // DataGridに薬品一覧を表示
            ChemicalDataGrid.ItemsSource = _chemicals;
        }

        private void LoadData()
        {
            _chemicals = _databaseManager.GetAllChemicals();
            _users = _databaseManager.GetAllUsers();
            _storageLocations = _databaseManager.GetAllStorageLocations();
        }

        private void btnToTESTView_Click(object sender, RoutedEventArgs e)
        {
            FirstViewPanel.Visibility = Visibility.Hidden;
            RegisterModePanel.Visibility = Visibility.Visible;
            LoadData();
            SetupUI();
        }


        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Panel1.Visibility == Visibility.Visible)
            {
                // 次のパネルへ移動
                Panel1.Visibility = Visibility.Hidden;
                Panel2.Visibility = Visibility.Visible;
                btnReturn.Visibility = Visibility.Visible;
            }
            else if (Panel2.Visibility == Visibility.Visible)
            {
                Panel2.Visibility = Visibility.Hidden;
                Panel3.Visibility = Visibility.Visible;
                btnNext.Content = "完了"; // ボタンのテキストを変更
            }
            else if (Panel3.Visibility == Visibility.Visible)
            {
                Panel3.Visibility = Visibility.Hidden;
                Panel1.Visibility = Visibility.Visible;
                btnNext.Content = "次へ"; // ボタンのテキストを元に戻す
                btnReturn.Visibility = Visibility.Hidden; // 戻るボタンを非表示にする
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (Panel2.Visibility == Visibility.Visible)
            {
                // 前のパネルへ戻る
                Panel2.Visibility = Visibility.Hidden;
                Panel1.Visibility = Visibility.Visible;
                btnReturn.Visibility = Visibility.Hidden; // 戻るボタンを非表示にする
            }
            else if (Panel3.Visibility == Visibility.Visible)
            {
                Panel3.Visibility = Visibility.Hidden;
                Panel2.Visibility = Visibility.Visible;
                btnNext.Content = "次へ"; // ボタンのテキストを元に戻す
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // ここでは処理しない（PreviewKeyDownで制御）
        }
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            // 今は処理が不要なら空のままでOK
        }

        //private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        e.Handled = true;

        //        var cellInfo = dataGrid.CurrentCell;
        //        int currentColumnIndex = cellInfo.Column.DisplayIndex;
        //        int currentRowIndex = dataGrid.Items.IndexOf(cellInfo.Item);

        //        // 編集確定
        //        dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
        //        //dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

        //        // 次のセルへ移動
        //        if (currentColumnIndex < dataGrid.Columns.Count - 1)
        //        {
        //            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[currentRowIndex], dataGrid.Columns[currentColumnIndex + 1]);
        //        }
        //        else
        //        {
        //            // 最後の列なら次の行へ
        //            if (currentRowIndex + 1 >= Reagents.Count)
        //            {
        //                //Items.Add(new RowData());
        //                //dataGrid.UpdateLayout(); // すぐに表示を反映
        //            }

        //            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[currentRowIndex + 1], dataGrid.Columns[0]);
        //        }
        //        dataGrid.SelectedItems.Clear();
        //        //dataGrid.SelectedCells.Add(dataGrid.CurrentCell);
        //        dataGrid.BeginEdit();
        //    }
        //}
        // 編集ボタンのイベントハンドラー
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.CommandParameter != null)
            {
                // CommandParameterに選択された行のデータオブジェクトが渡される
                var selectedReagent = button.CommandParameter;

                // ここに編集処理を記述
                // 例：編集ダイアログを開く、編集画面に遷移する等
                MessageBox.Show($"編集ボタンがクリックされました: {selectedReagent}");
            }
        }
        // 削除ボタンのイベントハンドラー
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.CommandParameter != null)
            {
                // CommandParameterに選択された行のデータオブジェクトが渡される
                var selectedReagent = button.CommandParameter;

                // 確認ダイアログを表示
                MessageBoxResult result = MessageBox.Show(
                    "この項目を削除しますか？",
                    "削除確認",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // ここに削除処理を記述
                    // 例：ViewModelのコレクションから削除等
                    MessageBox.Show($"削除ボタンがクリックされました: {selectedReagent}");
                }
            }
        }
    }
    
}
