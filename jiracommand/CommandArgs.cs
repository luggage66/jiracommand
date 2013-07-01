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
    [TabCompletion]
    public class CommandArgs
    {
        static string serverFile = ".jiraserver";
        static string tokenFile = ".token";

        [ArgDescription("Jira server. Can also be specified in config.")]
        public string Server { get; set; }

        [ArgDescription("Token to use for auth (normally not needed on the commandline)")]
        public string Token { get; set; }
        public string Project { get; set; }

        public CommandArgs()
        {
        }

        [ArgActionMethod, ArgDescription("gets an auth token from the server")]
        public void Login(LoginArgs args)
        {
            using (var jira = ConnectToJira())
            {
                string password = args.ResolvePassword();

                string token = jira.login(args.User, password);

                System.IO.File.WriteAllText(tokenFile, token);
            }
        }

        [ArgActionMethod, ArgDescription("Create a Jira issue")]
        public void AddIssue(AddIssueArgs args)
        {
            using (var jira = ConnectToJira())
            {
                var issue = new RemoteIssue();

                issue.summary = args.Summary;
                issue.description = args.ResolveDescription();
                issue.project = Project;
                issue.type = "3"; //task
                issue.priority = "2";

                if (args.Reporter != null && args.Reporter.Length > 0)
                    issue.reporter = args.Reporter;

                if (args.Assignee != null && args.Assignee.Length > 0)
                    issue.assignee = args.Assignee;

                if (args.Component != 0)
                    issue.components = new RemoteComponent[] { new RemoteComponent() { id = args.Component.ToString() } };

                issue = jira.createIssue(Token, issue);

                Console.WriteLine("A   {0}", issue.key);
            }
            
        }
        
        [ArgActionMethod, ArgDescription("See project options")]
        public void GetOptions(GetOptionsArgs args)
        {
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
            using (var jira = ConnectToJira())
            {
                jira.logout(Token);
                System.IO.File.Delete(tokenFile);
            }
        }

        JiraSoapServiceClient ConnectToJira()
        {
            if (Server == null || Server.Length < 1)
                Server = System.IO.File.ReadAllText(serverFile);

            if (Token == null || Token.Length < 1)
            {
                if (System.IO.File.Exists(tokenFile))
                    Token = System.IO.File.ReadAllText(tokenFile);
            }

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
        public string Assignee { get; set; }

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

        public string Password { get; set; }

        [ArgDescription("Password. You can skip and provide it interactively, too.")]
        public SecureStringArgument PasswordOnDemand { get; set; }

        public string ResolvePassword()
        {
            if (Password != null && Password.Length > 0)
                return Password;
            else
                return PasswordOnDemand.ConvertToNonsecureString();
        }
    }
}
