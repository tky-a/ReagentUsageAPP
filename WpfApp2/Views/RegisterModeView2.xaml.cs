using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DrugManagerApp.Views
{
    /// <summary>
    /// RegisterModeView2.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterModeView2 : Window
    {
        public ObservableCollection<RowData> Items { get; set; } = new();

        public RegisterModeView2()
        {
            InitializeComponent();
            dataGrid.ItemsSource = Items;
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
    public class RowData
    {
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string Class { get; set; } = "";
        public string UseStatus { get; set; } = "貸出可";
        public string Mass { get; set; } = "";
        public int LastUserID { get; set; }
        public string LastUseDate { get; set; } = "";
        public string FirstDate { get; set; } = "";
    }
}
