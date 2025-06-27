using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.ViewModels
{
    public class CoverViewModel
    {
        public RelayCommand GoToRegisterModeCommand { get; }

        public CoverViewModel(MainViewModel parent)
        {
            GoToRegisterModeCommand = new RelayCommand(() => parent.NavigateToRegisterMode());
        }
    }
}
