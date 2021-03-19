using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MDevice
    {
        public int Interval { get; set; }
        public ISensor Sensor { get; set; }
        public List<MAlert> Alerts { get; set; }

        public MDevice()
        {
            Alerts = new List<MAlert>();
        }

        public int CheckReading(MReading reading)
        {
            Regex rx = new Regex(@"\[.+?\]");
            
            foreach (MAlert alert in Alerts)
            {
                string toEval = alert.Rule;

                List<Match> ruleParams = rx.Matches(toEval).ToList();

                bool paramsApplied = true;

                foreach (Match m in ruleParams)
                {

                    string v = reading.Payload.Where(x => ("[" + x.Key + "]").ToUpper() == m.ToString().ToUpper()).Select(x => x.Value).FirstOrDefault();

                    if (v != null)
                    {
                        toEval = toEval.Replace(m.ToString(), v);
                    }
                    else
                    {
                        paramsApplied = false;
                    }
                }

                if (paramsApplied)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("expression", string.Empty.GetType(), toEval);
                    System.Data.DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    alert.Eval = bool.Parse((string)row["expression"]);
                }
                else
                {
                    alert.Eval = false;
                }

            }

            return Alerts.Where(x => x.Eval).Count();
        }
    }


}
