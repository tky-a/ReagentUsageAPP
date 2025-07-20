using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp2.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                // trueの場合はVisible
                // falseの場合はCollapsed (スペースを占めない)
                // Hiddenにしたい場合は Visibility.Hidden を返す
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 一方向バインディングで使うことが多いので、通常は実装不要
            throw new NotImplementedException();
        }
    }
}