using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace MistyDemo.Classes
{
    public class MDevice
    {
        public int Interval { get; set; }
        public ISensor Sensor { get; set; }
        public List<MAlert> Alerts { get; set; }
        public bool Alerted { get; set; }
        public bool Exposure { get; set; }

        public MDevice()
        {
            Alerts = new List<MAlert>();
        }

        public int CheckReading(MReading reading)
        {
            // GET [PLACEHOLDERS] FROM RULE...

            Regex rx = new Regex(@"\[.+?\]");

            // LOOP THROUGH DEVICE RULES, GET VALUES FROM READINGS AND EVALUATE THE RULE...
            // NO EVAL FUNCTION IN C# SO WE USE A DATATABLE AND PARSE THE SQL STRING AS A BOOL...

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

            int alertCount = Alerts.Where(x => x.Eval).Count();

            if (alertCount > 0)
            {
                Alerted = true;
            }

            return alertCount;
        }

        public void UpdatePins()
        {

            //GPIO FUNCTIONALITY ...

            var alertPin = Pi.Gpio[17];

            alertPin.PinMode = GpioPinDriveMode.Output;

            alertPin.Write(Alerted);

            var exposurePin = Pi.Gpio[18];

            exposurePin.PinMode = GpioPinDriveMode.Output;

            exposurePin.Write(Exposure);
        }

    }


}
