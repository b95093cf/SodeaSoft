using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SodeaSoft
{
    class Program
    {
        static List<Task> getData(SqliteConnection connection, DateTime from, DateTime to, int idTeam)
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
            AND IDUser IN (SELECT IDResource FROM Res_Team WHERE IDTeam == {idTeam})
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
                        reader.IsDBNull(3) ? new DateTime(1980, 1, 1) : reader.GetDateTime(3),
                        reader.IsDBNull(4) ? new DateTime(1980, 1, 1) : reader.GetDateTime(4),
                        reader.IsDBNull(5) ? -1 : reader.GetDouble(5),
                        reader.IsDBNull(8) ? reader.IsDBNull(7) ? "" : reader.GetString(7) : reader.GetString(8),
                        reader.IsDBNull(6) ? "" : reader.GetString(6)
                    ));
                }
            }
            return tasks;
        }

        static void printHelp()
        {
            Utils.prettyWriteLine($"Usage: ./PlanningPro-Extractor PlanningPro.db DateStart NumberOfWeeksToShow IDTeam", ConsoleColor.Yellow);
            Utils.prettyWriteLine("Ex: ./PlanningPro-Extractor 'C:/My Folder/PlanningPro.db' 2021-05-31 2 5", ConsoleColor.Yellow);
            Utils.prettyWriteLine("----------------------------------------------------------------------------", ConsoleColor.Yellow);
            Utils.prettyWriteLine(" - DateStart must be in the format YYYY-MM-DD and must be a monday", ConsoleColor.Yellow);
            Utils.prettyWriteLine(" - NumberOfWeeksToShow determines the number of output files generated (1 per week)", ConsoleColor.Yellow);
            Utils.prettyWriteLine(" - IDTeam is the id (integer) of the team to get from the table Res_Team", ConsoleColor.Yellow);
        }
        
        static void Main(string[] args)
        {
            int numberOfWeeks;
            int idTeam;
            DateTime startDate;
            if (args.Length != 4)
            {
                printHelp();
                Environment.ExitCode = 10;
            }
            else if (!File.Exists(args[0]))
            {
                Utils.prettyWriteLine($"File '{args[0]}' not found", ConsoleColor.Red);
                printHelp();
                Environment.ExitCode = 11;
            }
            else if (!DateTime.TryParseExact(args[1], "yyyy-M-d", null, System.Globalization.DateTimeStyles.None, out startDate))
            {
                Utils.prettyWriteLine("DateStart is in invalid format", ConsoleColor.Red);
                printHelp();
                Environment.ExitCode = 12;
            }
            else if (startDate.DayOfWeek != DayOfWeek.Monday)
            {
                Utils.prettyWriteLine("DateStart is not a monday", ConsoleColor.Red);
                printHelp();
                Environment.ExitCode = 13;
            }
            else if (!Int32.TryParse(args[2], out numberOfWeeks) || numberOfWeeks <= 0)
            {
                Utils.prettyWriteLine("Can't parse the number of weeks (must be above 0)", ConsoleColor.Red);
                printHelp();
                Environment.ExitCode = 14;
            }
            else if (!Int32.TryParse(args[3], out idTeam))
            {
                Utils.prettyWriteLine("Can't parse the id team (must be an integer)", ConsoleColor.Red);
                printHelp();
                Environment.ExitCode = 14;
            }
            else
            {
                string dataSourcePath = args[0];
                Utils.prettyWriteLine($"Opening database : {dataSourcePath}", ConsoleColor.Yellow);
                using (var connection = new SqliteConnection($"Data Source='{dataSourcePath}'"))
                {
                    connection.Open();
                    DateTime endDate = startDate.AddDays(5).AddSeconds(-1);
                    for (int i = 0; i < numberOfWeeks; i++)
                    {
                        DateTime currentStartDate = startDate.AddDays(i * 7);
                        DateTime currentEndDate = endDate.AddDays(i * 7);
                        Utils.prettyWriteLine($"{currentStartDate} -> {currentEndDate}", ConsoleColor.Green);
                        List<Task> tasks = getData(connection, startDate, currentEndDate, idTeam);
                        foreach (Task task in tasks)
                        {
                            Console.WriteLine(task);
                        }
                        string html = View.toHtml(tasks, currentStartDate, currentEndDate);
                        Utils.toHtml(html, $"{SodeaSoft.Properties.Resources.ResourceManager.GetString("OutputWeekFilePrefix")}{Utils.GetIso8601WeekOfYear(currentStartDate)}");
                    }
                }
            }
        }
    }
}
