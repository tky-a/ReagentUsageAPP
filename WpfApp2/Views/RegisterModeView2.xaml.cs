using DrugManagerApp.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrugManagerApp.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : Window
    {
        public ObservableCollection<ReagentModel> Items { get; set; } = new();



        public RegisterModeView2()
        {
            InitializeComponent();
            FirstViewPanel.Visibility = Visibility.Visible;
            RegisterModePanel.Visibility = Visibility.Hidden;
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
            dataGrid.ItemsSource = Items;
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
                    if (currentRowIndex + 1 >= Items.Count)
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
    }
    
}
