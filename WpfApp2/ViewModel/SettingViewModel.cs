using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.ViewModels
{
    public class SettingViewModel
    {
        public RelayCommand GoBackCommand { get; }

        public SettingViewModel(MainViewModel parent)
        {
            GoBackCommand = new RelayCommand(() => parent.NavigateToCover());
        }
    }
}
