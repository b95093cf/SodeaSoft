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
            command.CommandText = @$"
SELECT
    Code,
    Caption,
    Information,
    datStart,
    datEnd,
    Duration,
    RS,
    Nom,
    SurNom
FROM
(
    SELECT * FROM
    (
        SELECT * FROM
        (
            SELECT * FROM Task
            WHERE
            (
                (datStart <= '{properTo}' AND datEnd >= '{properFrom}') OR
                (datStart >= '{properFrom}' AND datStart <= '{properTo}')
            )
            AND Deleted is NULL
            AND IDUser IN (SELECT IDResource FROM Res_Team WHERE IDTeam == 5)
        ) R1
        LEFT OUTER JOIN Customer on R1.IDCustomer == Customer.ID
    ) R2
    LEFT OUTER JOIN Resource ON Resource.ID == R2.IDUser
)
";
            Utils.prettyWriteLine(command.CommandText, ConsoleColor.Yellow);
            Console.WriteLine();
            List<Task> tasks = new List<Task>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new Task(
                        reader.IsDBNull(0) ? "" : reader.GetString(0),
                        reader.IsDBNull(1) ? "" : reader.GetString(1),
                        reader.IsDBNull(2) ? "" : reader.GetString(2),
                        reader.IsDBNull(3) ? new DateTime(1980, 1, 1): reader.GetDateTime(3),
                        reader.IsDBNull(4) ? new DateTime(1980, 1, 1) : reader.GetDateTime(4),
                        reader.IsDBNull(5) ? -1 : reader.GetDouble(5),
                        reader.IsDBNull(8) ? reader.IsDBNull(7) ? "" : reader.GetString(7) : reader.GetString(8),
                        reader.IsDBNull(6) ? "" : reader.GetString(6)
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
                    DateTime today = DateTime.Now.Date;
                    //DateTime today = new DateTime(2019, 11, 27);
                    DateTime startDate;
                    DateTime endDate;
                    if (today.DayOfWeek == DayOfWeek.Sunday) startDate = today.AddDays(1);
                    else startDate = today.AddDays(DayOfWeek.Monday - today.DayOfWeek + 7);
                    endDate = startDate.AddDays(5).AddSeconds(-1);
                    for (int i = 0; i < numberOfWeeks; i++)
                    {
                        DateTime currentStartDate = startDate.AddDays(i * 7);
                        DateTime currentEndDate = endDate.AddDays(i * 7);
                        Utils.prettyWriteLine($"{currentStartDate} -> {currentEndDate}", ConsoleColor.Green);
                        List<Task> tasks = getData(connection, startDate, currentEndDate);
                        foreach (Task task in tasks)
                        {
                            Console.WriteLine(task);
                        }
                        string html = View.toHtml(tasks, currentStartDate, currentEndDate);
                        Utils.toPdf(html, $"Semaine{Utils.GetIso8601WeekOfYear(currentStartDate)}");
                    }
                }
            }
        }
    }
}
