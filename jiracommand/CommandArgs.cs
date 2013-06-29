using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace jiracommand
{
    [ArgExample("jiracommand config setserver \"https://...\"", "set a config property")]
    [ArgExample("jiracommand login -user bob -password \"123b$!!!\"", "login with a password on the commandline")]
    [ArgExample("jiracommand login -user dmull", "login with a password from a prompt")]
    public class CommandArgs
    {
        static string serverFile = ".jiraserver";

        [ArgActionMethod, ArgDescription("gets an auth token from the server")]
        public void Login(LoginArgs args)
        {
            Console.WriteLine(args.User);
            Console.WriteLine(args.Password.ConvertToNonsecureString());
        }

        [ArgActionMethod, ArgDescription("Create a Jira issue")]
        public void AddIssue(AddIssueArgs args)
        {
            Console.WriteLine("A   WPR-1234");
        }

        [ArgActionMethod, ArgDescription("See this screen")]
        public void Help(HelpArgs args)
        {
            ArgUsage.GetStyledUsage<CommandArgs>(null, new ArgUsageOptions() { ShowType = false }).Write();
        }

        [ArgActionMethod, ArgDescription("change setup")]
        public void Config(ConfigArgs args)
        {
            switch (args.Action)
            {
                case ConfigAction.setserver:
                    System.IO.File.WriteAllText(serverFile, args.Value);
                    break;
            }
        }


    }

    public class HelpArgs
    {
    }

    public class AddIssueArgs
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public string DescriptionFile { get; set; }
    }
    
    public class ConfigArgs
    {
        [ArgPosition(1)]
        public ConfigAction Action { get; set; }


        [ArgPosition(2)]
        public string Value { set; get; }
    }

    public enum ConfigAction
    {
        getserver,
        setserver
    }

    public class LoginArgs
    {
        public string User { get; set; }

        [ArgDescription("Password. You can skip and provide it interactively, too.")]
        public SecureStringArgument Password { get; set; }
    }
}
