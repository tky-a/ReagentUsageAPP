using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text;
using WpfApp2.Models;
using MaterialDesignThemes.Wpf;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;


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
        private int PanelNumber;
        private int _reagentCount = 0;

        public RegisterModeView2()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
            _databaseManager.EnsureTablesCreated();
            //DataContext = this;
            SetupFirstPanelUI();
        }

        private void SetupFirstPanelUI()
        {
            PanelNumber = 1;
            FirstViewPanel.Visibility = Visibility.Visible;
            RegisterModePanel.Visibility = Visibility.Hidden;
            HedderPanel.Visibility = Visibility.Hidden;
            ResultPanel.Visibility = Visibility.Hidden;
            btnToTESTMode.Focus();
        }

        private void FirstPanelEnterKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)

            {
                btnToTESTView_Click(sender, null);
                e.Handled = true;
            }
        }

        private void btnToTESTView_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            SetupRegisterModePanleUI();
        }


        private void SetupRegisterModePanleUI()
        {
            FirstViewPanel.Visibility = Visibility.Hidden;
            RegisterModePanel.Visibility = Visibility.Visible;
            HedderPanel.Visibility = Visibility.Visible;
            
            btnNext.IsEnabled = true;
            btnReturn.Visibility = Visibility.Hidden; // 初期状態では戻るボタンを非表示
            ChangeCircleColor(HedderMassEllipse, HedderMassText, Colors.Gray, Colors.LightGray);
            HedderFirstSecond.Background = new SolidColorBrush(Colors.Gray);
            ChangeCircleColor(HedderUserEllipse, HedderUserText, Colors.Gray, Colors.LightGray);
            HedderSecondThird.Background = new SolidColorBrush(Colors.Gray);
            InputBox.Focus();
        }

        private void LoadData()
        {
            _chemicals = _databaseManager.GetAllChemicals();
            _users = _databaseManager.GetAllUsers();
            _storageLocations = _databaseManager.GetAllStorageLocations();
        }


        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Enterキーが押されたらbtnNext_Clickと同じ処理を実行
                btnNext_Click(sender, null);
                e.Handled = true; // イベントを処理済みにする
            }
        }

        private void ChangeCircleColor(Ellipse ellipse, TextBlock text, Color fillColor, Color textColor)
        {
            ellipse.Fill = new SolidColorBrush(fillColor);
            text.Foreground = new SolidColorBrush(textColor);
        }

        private void SearchChemical_Click(Object sender, RoutedEventArgs? e)
        {
            if(int.TryParse(InputBox.Text, out int id))
            {
                var chemical = _databaseManager.GetChemicalById(id);
                //ReagentDataGrid.ItemsSource = null;
                if (chemical != null)
                {
                    ReagentDataGrid.ItemsSource = new List<Chemical> { chemical };
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            string uri;
            if (PanelNumber ==1)
            {
                uri = "pack://application:,,,/Resources/Images/weighing2.png";
                BitmapImage bitmap = new BitmapImage(new Uri(uri, UriKind.Absolute));
                ImgPanel.Source = bitmap;

                InputBox.SetValue(HintAssist.HintProperty, "質量を送信・入力");
                InputBox.SetValue(HintAssist.HelperTextProperty, "はかりにより送信ボタンを押す必要があります");

                SearchChemical_Click(sender, null);

                InputBox.Clear();
                btnReturn.Visibility = Visibility.Visible;
                PanelNumber = 2;
                InputBox.Focus();
                ChangeCircleColor(HedderMassEllipse, HedderMassText, Colors.MediumPurple, Colors.White);
                HedderFirstSecond.Background = new SolidColorBrush(Colors.MediumPurple);

            }
            else if (PanelNumber==2)
            {
                uri = "pack://application:,,,/Resources/Images/user_scan.png";
                BitmapImage bitmap = new BitmapImage(new Uri(uri, UriKind.Absolute));
                ImgPanel.Source = bitmap;

                InputBox.SetValue(HintAssist.HintProperty, "ユーザーIDを入力");
                InputBox.SetValue(HintAssist.HelperTextProperty, "数値を入力");
                InputBox.Clear();
                btnNext.Background = new SolidColorBrush(Colors.MediumPurple);
                btnNext.Foreground = new SolidColorBrush(Colors.White);
                btnNext.Content = "一時保存";
                PanelNumber = 3;
                InputBox.Focus();
                ChangeCircleColor(HedderUserEllipse, HedderUserText, Colors.MediumPurple, Colors.White);
                HedderSecondThird.Background = new SolidColorBrush(Colors.MediumPurple);
            }
            else if (PanelNumber ==3)
            {
                uri = "pack://application:,,,/Resources/Images/scan.png";
                BitmapImage bitmap = new BitmapImage(new Uri(uri, UriKind.Absolute));
                ImgPanel.Source = bitmap;

                InputBox.SetValue(HintAssist.HintProperty, "続けて記録できます");
                InputBox.SetValue(HintAssist.HelperTextProperty, "バーコードをスキャンするか入力します");
                InputBox.Clear();
                btnNext.Background = new SolidColorBrush(Colors.Transparent);
                btnNext.Foreground = new SolidColorBrush(Colors.Black);
                btnNext.Content = "次へ";
                btnReturn.Visibility = Visibility.Hidden;
                ChangeCircleColor(HedderMassEllipse, HedderMassText, Colors.Gray, Colors.LightGray);
                HedderFirstSecond.Background = new SolidColorBrush(Colors.Gray);
                ChangeCircleColor(HedderUserEllipse, HedderUserText, Colors.Gray, Colors.LightGray);
                HedderSecondThird.Background = new SolidColorBrush(Colors.Gray);
                _reagentCount++;
                ReagentCountText.Text = _reagentCount.ToString();
                ResultPanel.Visibility = Visibility.Visible;
                PanelNumber = 1;
                InputBox.Focus();
                ReagentDataGrid.ItemsSource = null;

            }
        }

        //private void btnReturn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Panel2.Visibility == Visibility.Visible)
        //    {
        //        // 前のパネルへ戻る
        //        Panel2.Visibility = Visibility.Hidden;
        //        Panel1.Visibility = Visibility.Visible;
        //        btnReturn.Visibility = Visibility.Hidden; // 戻るボタンを非表示にする
        //    }
        //    else if (Panel3.Visibility == Visibility.Visible)
        //    {
        //        Panel3.Visibility = Visibility.Hidden;
        //        Panel2.Visibility = Visibility.Visible;
        //        btnNext.Content = "次へ"; // ボタンのテキストを元に戻す
        //    }
        //}

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
