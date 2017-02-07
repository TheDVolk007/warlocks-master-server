using System;
using App.Common;
using App.Common.Wrappers;
using App.HttpServerScripts;
using App.HttpServerScripts.Implementations;
using Dal;
using Dal.Common;
using Domain;
using Moq;
using NUnit.Framework;


namespace UnitTestProject
{
    [TestFixture]
    public class MasterServerUnitTest
    {
        [Test]
        public void SuccessfulStartHttpServerWithServerInfoUpdate()
        {
            const string json =
                "{\"Name\":\"Unity Player Server\",\"IP\":\"localhost\",\"Port\":7777,\"Region\":8,\"PlayersCount\":0,\"PlayersMax\":100,\"MinutesToNextLevelChange\":120}";
            const string query = "/serverInfoUpdate";

            StartHttpServer(json, query);
        }

        [Test]
        public void SuccessfulStartHttpServerWithServerToConnectTo()
        {
            const string json = "{\"PersonId\":\"Player\",\"Nickname\":\"loh\",\"Country\":\"Peedorushka\"}";
            const string query = "/getServerToConnectTo";
            //Thread.Sleep(3000);
            StartHttpServer(json, query);
        }

        [Test]
        public void SuccessfulStopHttpServer()
        {
            //Thread.Sleep(1000);
            // Arange.
            var tcpListener = new Mock<ITcpListenerWrapper>();
            var streamFactory = new Mock<IStreamFactory>();
            var serverProvider = new Mock<IServersProvider>();
            var encryptor = new Mock<IEncryptor>();
            var stripMessager = new Mock<IStripMessenger>();
            var httpProcessorFactory = new Mock<IHttpProcessorFactory>();

            var masterServer = new MasterServerHttpServer(
                tcpListener.Object,
                streamFactory.Object,
                serverProvider.Object,
                httpProcessorFactory.Object,
                encryptor.Object,
                stripMessager.Object);

            // Act.
            // Assert.
            Assert.DoesNotThrow(() => masterServer.StopHttpServer());
        }

        private static void StartHttpServer(string json, string query)
        {
            // Arange.
            var tcpListener = new Mock<ITcpListenerWrapper>();
            var streamFactory = new Mock<IStreamFactory>();
            var serverProvider = new Mock<IServersProvider>();
            var encryptor = new Mock<IEncryptor>();
            var stripMessager = new Mock<IStripMessenger>();
            var httpProcessorFactory = new Mock<IHttpProcessorFactory>();
            var httpProcessor = new Mock<IHttpProcessor>();
            var memoryStream = new Mock<IMemoryStreamWrapper>();
            var streamWriter = new Mock<IStreamWriterWrapper>();

            var tcpClient = new Mock<ITcpClientWrapper>();
            tcpListener.Setup(t => t.AcceptTcpClient()).Returns(tcpClient.Object);

            Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> postHandler = delegate { };
            httpProcessorFactory.Setup(h => h.GetHttpProcessor(It.IsAny<ITcpClientWrapper>(),
                It.IsAny<IStreamFactory>(), It.IsAny<Action<string>>(),
                It.IsAny<Action<string, IMemoryStreamWrapper, IStreamWriterWrapper>>()))
                                .Callback((ITcpClientWrapper t, IStreamFactory s, Action<string> ag,
                                           Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> ah) => postHandler = ah)
                                .Returns(httpProcessor.Object);

            memoryStream.Setup(m => m.ToArray()).Returns(new byte[1]);
            
            encryptor.Setup(e => e.DecryptStringFromBytes(It.IsAny<byte[]>())).Returns(json);
            encryptor.Setup(e => e.EncryptStringToBytes(It.IsAny<string>())).Returns(new byte[1]);

            httpProcessor.Setup(h => h.Process())
                .Callback(() => postHandler(query, memoryStream.Object, streamWriter.Object));

            serverProvider.Setup(s => s.UpdateServer(It.IsAny<ServerInfo>())).ReturnsAsync(true);
            serverProvider.Setup(s => s.SelectServerForPlayerJson(It.IsAny<PlayerInfo>())).Returns("server");
            
            var masterServer = new MasterServerHttpServer(
                tcpListener.Object,
                streamFactory.Object,
                serverProvider.Object,
                httpProcessorFactory.Object,
                encryptor.Object,
                stripMessager.Object);

            // Act.
            // Assert.
            Assert.DoesNotThrow(() => masterServer.StartHttpServer());
        }
    }
}
