using Microsoft.Data.Sqlite;
using System;
using System.Globalization;
using System.IO;

namespace SodeaSoft
{
    public class Utils
    {
        private static void prettPrint(string txt, ConsoleColor color, bool newLine)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (newLine) Console.WriteLine(txt);
            else Console.Write(txt);
            Console.ForegroundColor = oldColor;
        }
        public static void prettyWrite(string txt, ConsoleColor color)
        {
            prettPrint(txt, color, false);
        }
        public static void prettyWriteLine(string txt, ConsoleColor color)
        {
            prettPrint(txt, color, true);
        }

        public static void showTables(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'sqlite_%'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);

                    Console.WriteLine($"Table: |{name}|");
                }
            }
        }

        public static void toPdf(string html, string path)
        {
            StreamWriter sr = new StreamWriter($"{path}.html");
            sr.Write(html);
            sr.Close();
            /*PdfDocument pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
            pdf.Save(path);*/
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
