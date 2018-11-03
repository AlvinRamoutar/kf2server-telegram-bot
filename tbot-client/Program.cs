using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tbot_client {
    class Program {
        static void Main(string[] args) {

            Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

            misc.ClientCredentials.UserName.UserName = "admin";
            misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            misc.Open();

            for(int i = 0; i < 10; i++) {
                misc.AdminSay("Module Test: " + (i + 1));
            }

        }
    }
}
