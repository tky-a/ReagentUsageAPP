using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : Window, INotifyPropertyChanged
    {
        private readonly DatabaseManager _databaseManager;
        public ObservableCollection<ReagentModel> Reagents { get; set; } 
        private ReagentModel _selectedReagent;

        //データベース関連
        public ReagentModel SelectedReagent
        {
            get => _selectedReagent;
            set
            {
                _selectedReagent = value;
                OnPropertyChanged();
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var reagents = await _databaseManager.LoadReagentsAsync();

                Reagents.Clear();
                foreach (var reagent in reagents)
                {
                    Reagents.Add(reagent);
                }
            }
            catch (Exception ex)
            {
                // エラーハンドリング（MessageBoxやログ出力など）
                MessageBox.Show($"データの読み込みエラー: {ex.Message}");
            }
        }
        public async Task SaveDataAsync()
        {
            try
            {
                await _databaseManager.SaveReagentsAsync(Reagents.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの保存エラー: {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //ここまでデータベース関連



        public RegisterModeView2()
        {
            InitializeComponent();
            DataContext = this;
            FirstViewPanel.Visibility = Visibility.Visible;
            RegisterModePanel.Visibility = Visibility.Hidden;
            _databaseManager = new DatabaseManager();
            Reagents = new ObservableCollection<ReagentModel>();
            LoadDataAsync();
        }

        private void btnToTESTView_Click(object sender, RoutedEventArgs e)
        {
            FirstViewPanel.Visibility = Visibility.Hidden;
            RegisterModePanel.Visibility = Visibility.Visible;
            RegisterViewMode2_Loaded(sender, e); // データグリッドの初期化
            btnNext.IsEnabled = true;
            btnReturn.Visibility = Visibility.Hidden; // 初期状態では戻るボタンを非表示
        }

        private void RegisterViewMode2_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = Reagents;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // ここでは処理しない（PreviewKeyDownで制御）
        }
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            // 今は処理が不要なら空のままでOK
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                var cellInfo = dataGrid.CurrentCell;
                int currentColumnIndex = cellInfo.Column.DisplayIndex;
                int currentRowIndex = dataGrid.Items.IndexOf(cellInfo.Item);

                // 編集確定
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                //dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                // 次のセルへ移動
                if (currentColumnIndex < dataGrid.Columns.Count - 1)
                {
                    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[currentRowIndex], dataGrid.Columns[currentColumnIndex + 1]);
                }
                else
                {
                    // 最後の列なら次の行へ
                    if (currentRowIndex + 1 >= Reagents.Count)
                    {
                        //Items.Add(new RowData());
                        //dataGrid.UpdateLayout(); // すぐに表示を反映
                    }

                    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[currentRowIndex + 1], dataGrid.Columns[0]);
                }
                dataGrid.SelectedItems.Clear();
                //dataGrid.SelectedCells.Add(dataGrid.CurrentCell);
                dataGrid.BeginEdit();
            }
        }
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
