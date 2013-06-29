using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace jiracommand
{
    public class CommandArgs
    {
        [ArgRequired]
        [ArgPosition(0)]
        [ArgDescription("")]
        public string Action { get; set; }

        public static void Help(HelpArgs args)
        {
            ArgUsage.GetStyledUsage<CommandArgs>().Write();
        }

        public static void Login(LoginArgs args)
        {
            Console.WriteLine(args.User);
            Console.WriteLine(args.Password.ConvertToNonsecureString());
        }

        public LoginArgs LoginArgs { get; set; }
        public HelpArgs HelpArgs { get; set; }
    }

    public class HelpArgs
    {
    }

    public class LoginArgs
    {
        public string User { get; set; }
        public SecureStringArgument Password { get; set; }
    }
}
