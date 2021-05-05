using System;
using System.Collections.Generic;
using System.Text;

namespace SodeaSoft
{
    public class ViewResource
    {
        public string ResourceName { get; }
        List<ViewTask> viewTasks = new List<ViewTask>();
        public ViewResource(string resourceName, List<Task> tasks)
        {
            ResourceName = resourceName;
            viewTasks = getTasksOfResource(resourceName, tasks);
        }
        public int getMaxNumberOfRows(DateTime startDate, DateTime endDate)
        {
            int max = 0;
            for (; startDate < endDate; startDate = startDate.AddDays(7))
            {
                int tmp = 0;
                foreach (ViewTask viewTask in viewTasks)
                    if (viewTask.taskIsActiveAtThisDate(startDate))
                        tmp++;
                if (tmp > max) max = tmp;
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
    }
}
