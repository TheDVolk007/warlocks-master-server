using System;
using System.Windows.Forms;
using App.Common;
using App.Common.Wrappers.Implementations;
using App.HttpServerScripts.Implementations;
using Dal;
using Dal.Common;
using Dal.Wrappers.Implementations;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Presentation
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var errorHandler = new ErrorHandler(log);

            var dbProvider = new MongoDbProvider(errorHandler);
            var defBuilder = new MongoDbDefinitionBuilder();
            var encryptor = new Encryptor();
            var serversProvider = new ServersProvider(dbProvider, defBuilder, encryptor);
            var tcpListener = new TcpListenerWrapper();
            var streamFactory = new StreamFactory();
            var stripMessenger = new StripMessenger(errorHandler);
            var httpProcessorFactory = new HttpProcessorFactory();

            var mainForm = new MainForm(
                serversProvider,
                new PlayersProvider(dbProvider, defBuilder, encryptor), 
                dbProvider,
                new MasterServerHttpServer(tcpListener,
                streamFactory,
                serversProvider,
                httpProcessorFactory,
                encryptor,
                stripMessenger),
                stripMessenger);

            Application.Run(mainForm);
        }
    }
}
