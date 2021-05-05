using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SodeaSoft
{
    class Program
    {
        static List<Task> getData(SqliteConnection connection, DateTime from, DateTime to)
        {
            string properFrom = from.ToString("yyyy-MM-dd");
            string properTo = to.ToString("yyyy-MM-dd");
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT Code, Caption, Information, datStart, datEnd, Duration, RS, Nom, SurNom  FROM ((SELECT * FROM Task LEFT OUTER JOIN Customer on Task.IDCustomer == Customer.ID) R1 LEFT OUTER JOIN Resource ON Resource.ID == R1.IDUser) WHERE (datStart <= '{properTo}' AND datEnd >= '{properFrom}') OR (datStart >= '{properFrom}' AND datStart <= '{properTo}')";
            Utils.prettyWriteLine(command.CommandText, ConsoleColor.Yellow);
            Console.WriteLine();
            List<Task> tasks = new List<Task>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new Task(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetDateTime(3),
                        reader.GetDateTime(4),
                        reader.GetDouble(5),
                        reader.GetString(7) != null ? reader.GetString(7) : reader.GetString(6)
                    ));
                }
            }
            return tasks;
        }
        
        static void Main(string[] args)
        {
            const int numberOfWeeks = 2;
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: SodeaSoft PlanningPro.db");
            }
            else if (!File.Exists(args[0]))
            {
                Console.WriteLine($"File '{args[0]}' not found");
            }
            else
            {
                string dataSourcePath = args[0];
                Utils.prettyWriteLine($"Opening database : {dataSourcePath}", ConsoleColor.Yellow);
                using (var connection = new SqliteConnection($"Data Source='{dataSourcePath}'"))
                {
                    connection.Open();
                    //showTables(connection);
                    DateTime today = DateTime.Now.Date.AddMonths(-24);
                    DateTime startDate;
                    DateTime endDate;
                    if (today.DayOfWeek == DayOfWeek.Sunday) startDate = today.AddDays(1);
                    else startDate = today.AddDays(DayOfWeek.Monday - today.DayOfWeek + 7);
                    endDate = startDate.AddDays(5).AddSeconds(-1);
                    for (int i = 0; i < numberOfWeeks; i++)
                    {
                        Utils.prettyWriteLine($"{startDate} -> {endDate}", ConsoleColor.Green);
                        List<Task> tasks = getData(connection, startDate, endDate);
                        foreach (Task task in tasks)
                        {
                            Console.WriteLine(task);
                        }
                        string html = View.toHtml(tasks, startDate, endDate);
                        Utils.toPdf(html, $"output{i}.pdf");
                        startDate.AddDays(7);
                        endDate.AddDays(7);
                    }
                }
            }
        }
    }
}
