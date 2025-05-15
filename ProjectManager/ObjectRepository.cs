using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    public static class ObjectRepository
    {
        private static bool dataProviderAlreadySet = false;
        private static IDataProvider dataProvider;
        public static IDataProvider DataProvider
        {
            get => dataProvider;
            set
            {
                if (dataProviderAlreadySet) return;
                else dataProvider = value;
                dataProviderAlreadySet = true;
            }
        }

        private static bool appConfigurationAlreadySet = false;
        private static AppConfiguration appConfiguration;
        public static AppConfiguration AppConfiguration
        {
            get => appConfiguration;
            set
            {
                if (appConfigurationAlreadySet) return;
                else appConfiguration = value;
                appConfigurationAlreadySet = true;
            }
        }

        private static bool notificationServiceAlreadySet = false;
        private static ISendNotification notificationService;
        public static ISendNotification NotificationService
        {
            get => notificationService;
            set
            {
                if (notificationServiceAlreadySet) return;
                else notificationService = value;
                notificationServiceAlreadySet = true;
            }
        }


        static ObjectRepository()
        {
            DataProvider = new RestDataProvider();
            AppConfiguration = new AppConfiguration();
            NotificationService = new EmailService();
        }
    }
}
