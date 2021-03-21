using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    // CLASS USED TO COLLECT ALERT DATA FROM TWIN, WITH ADDED 'EVAL' BOOL TO SHOW RESULTS OF THE RULE...
    public class MAlert
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Rule { get; set; }
        public string Alert { get; set; }
        public int Action { get; set; }
        public bool Eval { get; set; }

        public MAlert()
        {

        }
    }
}
