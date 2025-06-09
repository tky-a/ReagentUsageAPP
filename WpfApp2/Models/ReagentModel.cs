using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugManagerApp.Models
{
    public class ReagentModel
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
