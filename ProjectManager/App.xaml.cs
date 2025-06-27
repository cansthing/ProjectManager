using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string path = @"C:\Users\josua\source\repos\All big Projects\ProjectManager\ProjectManager\Properties\DBCS.txt";
        private static string dbConnectionString;
        public static string DBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(dbConnectionString))
                    dbConnectionString = System.IO.File.ReadAllText(path);
                return dbConnectionString;
            }
            set 
            { 
                dbConnectionString = value; 
                System.IO.File.WriteAllText(path, value);
            }
        }

    }
}
