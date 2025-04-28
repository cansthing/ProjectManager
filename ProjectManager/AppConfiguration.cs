using ProjectManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    public class AppConfiguration
    {
		private User currentUser;

		public User CurrentUser
		{
			get { return currentUser; }
			set { currentUser = value; }
		}


	}
}
