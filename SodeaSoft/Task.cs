using System;
using System.Collections.Generic;
using System.Text;

namespace SodeaSoft
{
    public class Task : IComparable<Task>
    {
        public string Code { get; }
        public string Caption { get; }
        public string Information { get; }
        public DateTime DatStart { get; }
        public DateTime DatEnd { get; }
        public double Duration { get; }
        public string Name { get; }
        public Task(string code, string caption, string information, DateTime datStart, DateTime datEnd, double duration, string name)
        {
            this.Code = code;
            this.Caption = caption;
            this.Information = information;
            this.DatStart = datStart;
            this.DatEnd = datEnd;
            this.Duration = duration;
            this.Name = name;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Code : {Code}\n");
            sb.Append($"Caption : {Caption}\n");
            sb.Append($"Information : {Information}\n");
            sb.Append($"DatStart : {DatStart.ToLongDateString()} {DatStart.ToLongTimeString()}\n");
            sb.Append($"DatEnd : {DatEnd.ToLongDateString()} {DatEnd.ToLongTimeString()}\n");
            sb.Append($"Duration : {Duration}\n");
            sb.Append($"Name : {Name}\n");
            return sb.ToString();
        }

        public int CompareTo(Task task)
        {
            if (task == null) return 1;
            else return Name.CompareTo(task.Name);
        }

        public static List<string> getDistinctResources(List<Task> tasks)
        {
            List<string> ret = new List<string>();
            foreach (Task task in tasks)
            {
                if (ret.FindIndex(v => v == task.Name) == -1)
                    ret.Add(task.Name);
            }
            return ret;
        }
    }
}
