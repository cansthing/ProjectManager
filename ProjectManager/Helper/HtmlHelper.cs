using ProjectManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Helper
{
    public abstract class HtmlHelper
    {
        public static string Greeting(User user)
        {
            return $"<p><strong>Hallo {user.Firstname}!</strong></p>";
        }
        public static string TableStyle()
        {
            return "<head><style>body {font-family: Arial, sans-serif;background-color: #f4f4f4;padding: 20px;}table {width: 100%;border-collapse: collapse;background: white;border-radius: 8px;overflow: hidden;box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);}\r\nth, td {padding: 12px;border: 1px solid #ddd;text-align: left;}\r\nth {background-color: #4CAF50;color: white;}\r\ntr:nth-child(even) {background-color: #f9f9f9;}\r\ntr:hover {background-color: #e3e3e3;}\r\n</style>\r\n</head>";
        }
        public static string Footer()
        {
            return "Mit freundlichen Grüßen,<br>" +
                "Das ProjektManagement System von Josua<br>" +
                "Diese Email wurde automatisch erstellt.</p>";
        }
        public static string Assignment(Assignment assignment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<p style=\"font-family: Arial, sans-serif;\">" +
                $"<strong>Titel: </strong>{assignment.Title}<br>" +
                $"<strong>Beschreibung: </strong>{assignment.Description}<br>" +
                $"<strong>Erstellungsdatum: </strong>{assignment.AssignDate.ToString("dd.MM.yyyy")}<br>" +
                $"<strong>Fälligkeitsdatum: </strong>{assignment.DueDate.ToString("dd.MM.yyyy")}<br>" +
                $"<strong>Priorität: </strong>{assignment.Priority}<br><br>" +

                $"Diese Aufgabe gehört zu dem Projekt <strong>{assignment.Project.Title}</strong>. Hier sind die Details:<br>" +
                $"<strong>Projektbeschreibung: </strong>{assignment.Project.Description}<br>" +
                $"<strong>Erstellungsdatum: </strong>{assignment.Project.Start.ToString("dd.MM.yyyy")}<br>" +
                $"<strong>Verantwortlich: </strong>{assignment.Project.Responsibility.Fullname}<br></p>");
            return sb.ToString();
        }
        private static string AssignmentsTable(ObservableCollection<Assignment> assignments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table border='1'><tr><th>Projekt</th><th>Titel</th><th>Beschreibung</th><th>Erstellungsdatum</th><th>Fälligkeitsdatum</th><th>Fortschritt</th></tr>");

            foreach (var assignment in assignments)
            {
                sb.Append($"<tr><td>{assignment.Project?.Title}</td><td>{assignment.Title}</td><td>{assignment.Description}</td><td>{assignment.AssignDate.ToString("dd.MM.yyyy")}</td><td>{assignment.DueDate.ToString("dd.MM.yyyy")}</td><td>{assignment.ProgressPercent}%</td></tr>");
            }
            sb.Append("</table><br>");
            return sb.ToString();
        }
        public static string CurrentAssignments(ObservableCollection<Assignment> assignments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HtmlHelper.TableStyle());
            sb.Append("<h3>Aktuelle Aufgaben</h3>" +
                $"<p>Hier ist einmal eine Auflistung der aktuellen Aufgaben, die du noch zu erledigen hast.</p>");
            sb.Append(HtmlHelper.AssignmentsTable(assignments));
            return sb.ToString();
        }

    }
}
