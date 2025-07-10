using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => execute();

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
