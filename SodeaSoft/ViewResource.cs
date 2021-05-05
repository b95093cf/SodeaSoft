using System;
using System.Collections.Generic;
using System.Text;

namespace SodeaSoft
{
    public class ViewResource
    {
        public string ResourceName { get; }
        List<ViewTask> viewTasks = new List<ViewTask>();
        public Week week;
        public ViewResource(string resourceName, List<Task> tasks, DateTime startDate, DateTime endDate)
        {
            ResourceName = resourceName;
            viewTasks = getTasksOfResource(resourceName, tasks);
            week = new Week(startDate, endDate, viewTasks);
        }
        public int getMaxNumberOfRows(DateTime startDate, DateTime endDate)
        {
            int max = 0;
            foreach (List<ViewTask> day in week.days)
            {
                if (day.Count > max) max = day.Count;
            }
            return max;
        }
        public static List<ViewTask> getTasksOfResource(string resourceName, List<Task> tasks)
        {
            List<ViewTask> ret = new List<ViewTask>();
            foreach (Task task in tasks)
                if (task.Name == resourceName)
                    ret.Add(new ViewTask(task.Caption, task.Name, task.Information, task.DatStart, task.DatEnd));
            return ret;
        }
        public class Week
        {
            public List<List<ViewTask>> days = new List<List<ViewTask>>();

            public Week(DateTime startDate, DateTime endDate, List<ViewTask> viewTasks)
            {
                int i = 0;
                for (DateTime it = startDate.Date; it <= endDate.Date; it = it.AddDays(1))
                {
                    List<ViewTask> day = new List<ViewTask>();
                    foreach (ViewTask viewTask in viewTasks)
                        if (viewTask.taskIsActiveAtThisDate(it))
                            day.Add(viewTask);
                    i++;
                    days.Add(day);
                }
            }
        }
    }
}
