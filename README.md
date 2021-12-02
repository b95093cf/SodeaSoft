# SodeaSoft PlanningPro Extractor

The program extracts tasks that should be started by a specific team for the next n weeks.
Then it creates n html plannings: 
- 1 column containing the "resource" (name of the user)
- 5 column for each day of the week containing the tasks for that day for the correponding resource

## Usage

./PlanningPro-Extractor *pathToPlanningPro.db* *DateStart* *NumberOfWeeksToShow* *IDTeam* 

- *DateStart* must be in the format YYYY-MM-DD and must be a monday  
- *NumberOfWeeksToShow* determines the number of output files generated (1 per week)  
- *IDTeam* is the id (integer) of the team located in the table Res_Team

Ex: **./PlanningPro-Extractor "C:/My Folder/PlanningPro.db" 2021-05-31 2 5**  

## Dependencies

+ [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)  
