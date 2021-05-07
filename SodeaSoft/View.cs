using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SodeaSoft
{
    public class View
    {
		private static string getTHead(DateTime startDate, DateTime endDate)
        {
			StringBuilder sb = new StringBuilder();
			sb.Append("<thead>");
			sb.Append($"<tr><th colspan=6>Semaine {Utils.GetIso8601WeekOfYear(startDate)} ({startDate.ToShortDateString()} -> {endDate.ToShortDateString()})</th></tr>");
			sb.Append("<tr><th>Resource</th>");
			for (int i = 0; i < 5; i++)
			{
				sb.Append($"<th>{ startDate.AddDays(i).ToString("dddd M") }</th>"); //affiche numéro du mois
			}
			sb.Append("</tr>");
			sb.Append("</thead>");
			return sb.ToString();
		}
		public static string toHtml(List<Task> tasks, DateTime startDate, DateTime endDate)
		{
			List<string> resources = Task.getDistinctResources(tasks);
			List<ViewResource> viewResources = new List<ViewResource>();
			foreach (string resource in resources)
				viewResources.Add(new ViewResource(resource, tasks, startDate, endDate));
			StringBuilder sb = new StringBuilder();
			string head = File.ReadAllText("www/head.html");
			string foot = File.ReadAllText("www/foot.html");
			sb.Append(head);
			sb.Append(@"<table class='m'>");
			sb.Append(getTHead(startDate, endDate)); // tHead
			sb.Append(@"<tbody>");
			// TBODY START
			bool firstResourcePass = true;
			foreach (ViewResource viewResource in viewResources)
			{
				if (!firstResourcePass)
					sb.Append("<tr><td class='sep' colspan=6></td></tr>");
				int numberOfRows = viewResource.getMaxNumberOfRows(startDate, endDate);
				for (int i = 0; i < numberOfRows; i++)
				{
					sb.Append("<tr>");
					for (int j = 0; j < viewResource.week.days.Count; j++)
					{
						if (i == 0 && j == 0)
                        {
							sb.Append($"<td rowspan={numberOfRows}><pre>{viewResource.ResourceName}</pre></td>");
                        }
						sb.Append("<td>");
						List<ViewTask> viewtasksOfTheDay = viewResource.week.days[j];
						if (i < viewtasksOfTheDay.Count)
						{
							ViewTask currentViewTask = viewtasksOfTheDay[i];
							sb.Append($"<table><tr><td><pre>{currentViewTask.Caption}</pre></td><td><pre>{currentViewTask.RS}</pre></td></tr><tr><td colspan=2><pre>{currentViewTask.Information}</pre></td></tr></table>");
							// OK
                        }
						else
                        {
							// Empty :)
						}
						sb.Append("</td>");
					}
					sb.Append("</tr>");
				}
            }
			// TBODY END
			sb.Append("</tbody>");
			sb.Append("</table>");
			sb.Append(foot);
			return sb.ToString();
		}
	}
}
