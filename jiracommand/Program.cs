using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;
using log4net;

namespace jiracommand
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("jiracommand.logging.config.xml"));

            ILog commandLog = LogManager.GetLogger("ExecutedCommands");
            bool commandSuccessful = false;
            try
            {

                Args.InvokeAction<CommandArgs>(args);
                commandSuccessful = true;
            }
            catch (PowerArgs.ArgException argEx)
            {
                Console.WriteLine(argEx.Message);
                ArgUsage.GetStyledUsage<CommandArgs>(null, new ArgUsageOptions() { ShowType = false }).Write();
            }
            finally
            {
                if (commandSuccessful)
                    commandLog.InfoFormat("#{0}", string.Join(" ", args));
                else
                    commandLog.Info(string.Join(" ", args));
            }

        }
    }
}
