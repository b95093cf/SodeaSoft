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
			sb.Append($"<tr><th colspan=11>Semaine {Utils.GetIso8601WeekOfYear(startDate)} ({startDate.ToShortDateString()} -> {endDate.ToShortDateString()})</th></tr>");
			sb.Append("<tr><th>Resource</th>");
			for (int i = 0; i < 5; i++)
			{
				sb.Append($"<th colspan=2>{ startDate.AddDays(i).ToString("M") }</th>");
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
					sb.Append("<tr><td class='sep' colspan=11></td></tr>");
				bool firstTaskPass = true;
				for (DateTime dateIterator = startDate.Date; dateIterator <= endDate.Date; dateIterator = dateIterator.AddDays(1))
                {
					DateTime datStart = dateIterator.Date;
					DateTime datEnd = dateIterator.Date.AddDays(1).AddSeconds(-1);
					int maxNumberOfRows = viewResource.getMaxNumberOfRows(datStart, datEnd);
					if (firstTaskPass) {
						sb.Append($"<td rowspan={maxNumberOfRows * 3 - 1}><pre>{viewResource.ResourceName}</pre></td>");
					}
					sb.Append($"<td><pre></pre></td><td><pre></pre></td>");
					sb.Append($"<td><pre></pre></td><td><pre></pre></td>");
					sb.Append($"<td><pre></pre></td><td><pre></pre></td>");
					sb.Append($"<td><pre></pre></td><td><pre></pre></td>");
					sb.Append($"<td><pre></pre></td><td><pre></pre></td>");
					firstTaskPass = false;
				}
				firstResourcePass = false;
            }
			// TBODY END
			sb.Append("</tbody>");
			sb.Append("</table>");
			sb.Append(foot);
			return sb.ToString();
		}
	}
}
