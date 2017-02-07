using System;
using System.Threading;
using App.Common;
using App.Common.Wrappers;
using Dal;
using Dal.Common;
using Domain;
using Newtonsoft.Json;


namespace App.HttpServerScripts.Implementations
{
    public class MasterServerHttpServer : HttpServer
    {
        private readonly IServersProvider serversProvider;
        private readonly IEncryptor encryptor;
        private readonly IStripMessenger stripMessenger;
        private static Thread httpServerThread;
        
        public MasterServerHttpServer(ITcpListenerWrapper tcpListener,
            IStreamFactory streamFactory,
            IServersProvider serversProvider,
            IHttpProcessorFactory httpProcessorFactory,
            IEncryptor encryptor,
            IStripMessenger stripMessenger)
            : base(tcpListener, streamFactory, httpProcessorFactory)
        {
            this.serversProvider = serversProvider;
            this.encryptor = encryptor;
            this.stripMessenger = stripMessenger;
        }

        public override void StartHttpServer()
        {
            if (httpServerThread != null)
            {
                httpServerThread.Abort();
                Thread.Sleep(250);
            }
            httpServerThread = new Thread(Listen);
            httpServerThread.Start();
        }

        public override void StopHttpServer()
        {
            Stop();
            if(httpServerThread != null)
            {
                httpServerThread.Abort();
            }
        }

        protected override void HandleGETRequest(string httpUrl)
        {
            //Console.WriteLine("request: {0}", p.httpUrl);
            //p.WriteSuccess();
            //p.outputStream.WriteLine("<html><body><h1>test server</h1>");
        }

        protected override async void HandlePOSTRequest(string httpUrl, IMemoryStreamWrapper ms, IStreamWriterWrapper outputStream)
        {
            var json = encryptor.DecryptStringFromBytes(ms.ToArray());

            switch (httpUrl)
            {
                case "/serverInfoUpdate":
                {
                    var serverInfo = JsonConvert.DeserializeObject<ServerInfo>(json);
                    var serverWasUpdated = await serversProvider.UpdateServer(serverInfo);
                    if(!serverWasUpdated)
                    {
                        stripMessenger.StripMessage =
                            $"Added new server: {serverInfo.Name} - {serverInfo.Ip}:{serverInfo.Port} ({serverInfo.Region})";
                    }
                    return;
                }
                
                case "/getServerToConnectTo":
                {
                    var playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(json);
                    var serverJson = serversProvider.SelectServerForPlayerJson(playerInfo);
                    var serverInfoBytes = encryptor.EncryptStringToBytes(serverJson);
                    outputStream.WriteThroughBaseStream(serverInfoBytes, 0, serverInfoBytes.Length);
                    stripMessenger.StripMessage = "Query 'getServerToConnectTo' have been successfully processed";
                    return;
                }
            }

            throw new Exception("Unknown query");
        }
    }
}
