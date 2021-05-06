using System;
using System.Collections.Generic;
using System.Text;

namespace SodeaSoft
{
    public class ViewTask
    {
        public string Caption { get; }
        public string Name { get; }
        public string RS { get; }
        public string Information { get; }
        private DateTime datStart;
        private DateTime datEnd;
        public ViewTask(string caption, string name, string rs, string information, DateTime datStart, DateTime datEnd)
        {
            Caption = caption;
            Name = name;
            RS = rs;
            Information = information;
            this.datStart = datStart;
            this.datEnd = datEnd;
        }
        public bool taskIsActiveAtThisDate(DateTime date)
        {
            return datStart.Date <= date.Date && date.Date <= datEnd.Date;
        }
    }
}
