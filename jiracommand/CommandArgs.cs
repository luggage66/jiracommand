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
        static string tokenFile = ".token";

        [ArgDescription("Jira server. Can also be specified in config.")]
        public string Server { get; set; }
        public string Token { get; set; }
        public string Project { get; set; }

        public CommandArgs()
        {
        }

        void PreCommand()
        {
            if (Server == null || Server.Length < 1)
                Server = System.IO.File.ReadAllText(serverFile);

            if (Token == null || Token.Length < 1)
            {
                if (System.IO.File.Exists(tokenFile))
                    Token = System.IO.File.ReadAllText(tokenFile);
            }
        }

        [ArgActionMethod, ArgDescription("gets an auth token from the server")]
        public void Login(LoginArgs args)
        {
            PreCommand();

            using (var jira = ConnectToJira())
            {
                string token = jira.login(args.User, args.Password.ConvertToNonsecureString());
                System.IO.File.WriteAllText(tokenFile, token);
            }
        }

        [ArgActionMethod, ArgDescription("Create a Jira issue")]
        public void AddIssue(AddIssueArgs args)
        {
            PreCommand();

            using (var jira = ConnectToJira())
            {
                var issue = new RemoteIssue();

                issue.summary = args.Summary;
                issue.description = args.ResolveDescription();
                issue.project = Project;
                issue.type = "3"; //task
                issue.priority = "2";

                if (args.Component != 0)
                    issue.components = new RemoteComponent[] { new RemoteComponent() { id = args.Component.ToString() } };

                issue = jira.createIssue(Token, issue);

                Console.WriteLine("A   {0}", issue.key);
            }
            
        }
        
        [ArgActionMethod, ArgDescription("See project options")]
        public void GetOptions(GetOptionsArgs args)
        {
            PreCommand();

            using (var jira = ConnectToJira())
            {
                var components = jira.getComponents(Token, Project);

                Console.WriteLine("Components:");
                foreach (var c in components)
                {
                    Console.WriteLine("{0}: {1}", c.id, c.name);
                }
            }
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

        [ArgActionMethod]
        public void Logout(LogoutArgs args)
        {
            PreCommand();

            using (var jira = ConnectToJira())
            {
                jira.logout(Token);
                System.IO.File.Delete(tokenFile);
            }
        }

        JiraSoapServiceClient ConnectToJira()
        {
            return new JiraSoapServiceClient("jirasoapservice-v2", Server);
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
        public string Reporter { get; set; }

        [ArgDescription("The component ID (see getoptions).")]
        public int Component { get; set; }

        public string ResolveDescription()
        {
            if (DescriptionFile != null && DescriptionFile.Length > 0)
                return System.IO.File.ReadAllText(DescriptionFile);
            else
                return Description;
        }
    }

    public class GetOptionsArgs
    {
    }
    
    public class ConfigArgs
    {
        [ArgPosition(1)]
        public ConfigAction Action { get; set; }


        [ArgPosition(2)]
        public string Value { set; get; }
    }

    public class LogoutArgs
    {
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
