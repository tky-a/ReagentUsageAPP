using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.ViewModel
{
    public class InputSetViewModel
    {
        public InputSet InputSet { get; }
        public string UserName { get; }

        public InputSetViewModel(InputSet inputSet, User user)
        {
            InputSet = inputSet;
            UserName = user?.UserName ?? inputSet.InputUserId.ToString();
        }
    }
}
