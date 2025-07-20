using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WpfApp2.ViewModel
{
    public partial class GeneralSettingViewModel: ObservableObject
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        public List<ThemeOption> ThemeOptions { get; } = new List<ThemeOption>
        {
            new ThemeOption { Mode = ThemeMode.Light, DisplayName="ライトモード" },
            new ThemeOption { Mode = ThemeMode.Dark, DisplayName="ダークモード" },
            new ThemeOption { Mode = ThemeMode.System, DisplayName="システムに従う" }
        };

        [ObservableProperty]
        private ThemeMode _selectedThemeMode;

    }
}
