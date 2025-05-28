using DrugManagerApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Helpers;
using WpfApp2.Models;

namespace WpfApp2.Views
{
    /// <summary>
    /// RegisterModeView.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView : Window
    {
        private bool _isSearchOpen = true;
        private DatabaseManager _databaseManager;
        private int _currentStep;
        private bool _isSearchPanelExpanded = true;
        private DrugItem _currentDrugItem;
        

        public RegisterModeView()
        {
            InitializeComponent();
            InitializeComponents();
            LoadData();
        }
        private void InitializeComponents()
        {
            _databaseManager = new DatabaseManager();
            _currentDrugItem = new DrugItem();

            ShowStep(1);
            UpdateStepIcons();
            ToggleSearchPanel();

            // テキストボックスのイベント設定
            ScanInputBox.KeyDown += OnScanInputKeyDown;
            WeightInputBox.KeyDown += OnWeightInputKeyDown;
            UserIdInputBox.KeyDown += OnUserIdInputKeyDown;
            SearchTextBox.KeyDown += OnSearchTextKeyDown;
        }

        private void LoadData()
        {
            try
            {
            }
            catch(Exception ex)
            {
            }
        }

        #region ステップ管理

        private void ShowStep(int stepNumber)
        {
            // すべてのステップパネルを非表示
            Step1Panel.Visibility = Visibility.Hidden;
            Step2Panel.Visibility = Visibility.Hidden;
            Step3Panel.Visibility = Visibility.Hidden;
            Step4Panel.Visibility = Visibility.Hidden;
            // 指定されたステップのパネルを表示
            switch (stepNumber)
            {
                case 1:
                    Step1Panel.Visibility = Visibility.Visible;
                    ScanInputBox.Focus();
                    break;
                case 2:
                    Step2Panel.Visibility = Visibility.Visible;
                    WeightInputBox.Focus();
                    break;
                case 3:
                    Step3Panel.Visibility = Visibility.Visible;
                    UserIdInputBox.Focus();
                    break;
                case 4:
                    Step4Panel.Visibility = Visibility.Visible;
                    break;
            }

            _currentStep = stepNumber;
            UpdateStepIcons();
        }

        private void UpdateStepIcons()
        {
            // アイコンの色をリセット
            ScanIcon.Foreground = System.Windows.Media.Brushes.Gray;
            MassIcon.Foreground = System.Windows.Media.Brushes.Gray;
            UserIcon.Foreground = System.Windows.Media.Brushes.Gray;

            ScanText.Foreground = System.Windows.Media.Brushes.Gray;
            MassText.Foreground = System.Windows.Media.Brushes.Gray;
            UserText.Foreground = System.Windows.Media.Brushes.Gray;
            // 現在のステップをハイライト
            switch (_currentStep)
            {
                case 1:
                    ScanIcon.Foreground = System.Windows.Media.Brushes.Blue;
                    ScanText.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case 2:
                    MassIcon.Foreground = System.Windows.Media.Brushes.Blue;
                    MassText.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case 3:
                    UserIcon.Foreground = System.Windows.Media.Brushes.Blue;
                    UserText.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
            }
        }

        #endregion

        #region イベントハンドラ

        private void OnScanInputKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ProcessScanInput();
            }
        }

        private void OnWeightInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessWeightInput();
            }
        }

        private void OnUserIdInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserIdInput();
            }
        }

        private void OnSearchTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }
        private void btnTEST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrontCoverView window = new FrontCoverView();
                window.Show();//NOTmodal
                //window.ShowDialog();//modal
            }
            catch
            {
                return;
            }
            //// テスト用：次のステップに進む
            //if (_currentStep < 3)
            //{
            //    ShowStep(_currentStep + 1);
            //}
            //else
            //{
            //    CompleteLendingProcess();
            //}
        }
        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == ResultItemBTN)
            {
                ResultItemIdBox.IsReadOnly = false;
                ResultItemIdBox.Focus();
            }
            else if (button == ResultWeightBTN)
            {
                ResultWeightBox.IsReadOnly = false;
                ResultWeightBox.Focus();
            }
            else if (button == ResultUserIdBTN)
            {
                ResultUserIdBox.IsReadOnly = false;
                ResultUserIdBox.Focus();
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }
        private void ToggleSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSearchPanel();
        }
        #endregion

        #region 処理メソッド

        private void ProcessScanInput()
        {
            string scanInput = ScanInputBox.Text.Trim();

            if (string.IsNullOrEmpty(scanInput))
            {
                MessageBox.Show("管理番号を入力してください。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationHelper.IsValidItemId(scanInput))
            {
                MessageBox.Show("管理番号の形式が正しくありません。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var drugItem = _databaseManager.GetDrugItemById(scanInput);
                if (drugItem != null)
                {
                    _currentDrugItem = drugItem;
                    ResultItemIdBox.Text = scanInput;
                    ShowStep(2);
                }
                else
                {
                    MessageBox.Show("該当する薬品が見つかりません。", "検索エラー",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データベースエラー: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessWeightInput()
        {
            string weightInput = WeightInputBox.Text.Trim();

            if (string.IsNullOrEmpty(weightInput))
            {
                MessageBox.Show("質量を入力してください。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationHelper.IsValidWeight(weightInput))
            {
                MessageBox.Show("質量の形式が正しくありません。数値で入力してください。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentDrugItem.CurrentWeight = double.Parse(weightInput);
            ResultWeightBox.Text = weightInput;
            ShowStep(3);
        }

        private void ProcessUserIdInput()
        {
            string userIdInput = UserIdInputBox.Text.Trim();

            if (string.IsNullOrEmpty(userIdInput))
            {
                MessageBox.Show("利用者IDを入力してください。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationHelper.IsValidUserId(userIdInput))
            {
                MessageBox.Show("利用者IDの形式が正しくありません。", "入力エラー",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentDrugItem.UserId = userIdInput;
            ResultUserIdBox.Text = userIdInput;

            CompleteLendingProcess();
        }

        private void CompleteLendingProcess()
        {
            try
            {
                _currentDrugItem.LendingDate = DateTime.Now;
                _currentDrugItem.IsLent = true;

                _databaseManager.UpdateDrugItem(_currentDrugItem);

                MessageBox.Show("貸出処理が完了しました。", "完了",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"貸出処理でエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PerformSearch()
        {
            string searchQuery = SearchTextBox.Text.Trim();

            try
            {
                List<DrugItem> searchResults;

                if (string.IsNullOrEmpty(searchQuery))
                {
                    return;
                }
                else
                {
                    searchResults = _databaseManager.SearchDrugItems(searchQuery);
                }

                var sb = new StringBuilder();
                foreach(var item in searchResults)
                {
                    sb.AppendLine($"管理番号: {item.管理番号}, \n薬品名: {item.薬品名}, \n現在量: {item.CurrentWeight}");
                }
                SearchResultsTextBlock.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"検索でエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleSearchPanel()
        {
            if (_isSearchPanelExpanded)
            {
                SearchColumn.Width = new GridLength(20);
                ToggleSearchButton.Content = "<";
                _isSearchPanelExpanded = false;
            }
            else
            {
                SearchColumn.Width = new GridLength(200);
                ToggleSearchButton.Content = ">";
                _isSearchPanelExpanded = true;
            }
        }

        private void ResetForm()
        {
            // フォームをリセット
            ScanInputBox.Text = "";
            WeightInputBox.Text = "";
            UserIdInputBox.Text = "";
            ResultItemIdBox.Text = "";
            ResultWeightBox.Text = "";
            ResultUserIdBox.Text = "";

            _currentDrugItem = new DrugItem();
            ShowStep(1);

            // 検索結果を更新
            LoadData();
        }

        #endregion

        #region ウィンドウ操作

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void CloseButton_MouseEnterLeave(object sender, MouseEventArgs e)
        {
            //if (e.RoutedEvent.Name == "MouseEnter")
            //{
            //    CloseButton.Background = System.Windows.Media.Brushes.Red;
            //}
            //else if(e.RoutedEvent.Name == "MouseLeave")
            //{
            //    CloseButton.Background = System.Windows.Media.Brushes.Transparent;
            //}
        }

        #endregion
    }
}