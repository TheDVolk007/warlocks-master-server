using System;
using System.IO;
using App.Common.Wrappers;
using App.HttpServerScripts.Implementations;
using Moq;
using NUnit.Framework;


namespace UnitTestProject
{
    [TestFixture]
    public class HttpProcessorTest
    {
        [Test]
        public void SuccessfulProcessPostQuery()
        {
            // Arange.
            var tcpClient = new Mock<ITcpClientWrapper>();
            var streamFactory = new Mock<IStreamFactory>();
            var stream = new Mock<IStreamWrapper>();
            var streamWriter = new Mock<IStreamWriterWrapper>();
            var memoryStream = new Mock<IMemoryStreamWrapper>();

            tcpClient.Setup(t => t.GetStream()).Returns(stream.Object);
            streamFactory.Setup(s => s.GetStreamWriterWrapper(It.IsAny<IStreamWrapper>())).Returns(streamWriter.Object);
            streamFactory.Setup(s => s.GetMemoryStreamWrapper()).Returns(memoryStream.Object);

            var amountOfUsesStreamReadLine = -1;
            stream.Setup(s => s.StreamReadLine()).Returns(() =>
            {
                amountOfUsesStreamReadLine++;
                return GetStreamReadLineResponse(amountOfUsesStreamReadLine);
            });
            stream.Setup(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(136);

            var getHandlerInvokingCount = 0;
            var getHandler = new Action<string>(s => getHandlerInvokingCount++);

            var postHandlerInvokingCount = 0;
            var postHandler = new Action<string, IMemoryStreamWrapper, IStreamWriterWrapper>((s, m, sw) => postHandlerInvokingCount++);

            var httpProc = new HttpProcessor(tcpClient.Object, streamFactory.Object, getHandler, postHandler);

            // Act.
            httpProc.Process();

            // Assert.
            memoryStream.Verify(m => m.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            memoryStream.Verify(m => m.Seek(It.IsAny<long>(), It.IsAny<SeekOrigin>()), Times.Once);
            Assert.AreEqual(0, getHandlerInvokingCount);
            Assert.AreEqual(1, postHandlerInvokingCount);
        }

        [Test]
        public void UnsuccessfulProcessPostQuery()
        {
            // Arange.
            var tcpClient = new Mock<ITcpClientWrapper>();
            var streamFactory = new Mock<IStreamFactory>();
            var stream = new Mock<IStreamWrapper>();
            var streamWriter = new Mock<IStreamWriterWrapper>();
            var memoryStream = new Mock<IMemoryStreamWrapper>();

            tcpClient.Setup(t => t.GetStream()).Returns(stream.Object);
            streamFactory.Setup(s => s.GetStreamWriterWrapper(It.IsAny<IStreamWrapper>())).Returns(streamWriter.Object);
            streamFactory.Setup(s => s.GetMemoryStreamWrapper()).Returns(memoryStream.Object);
            
            stream.Setup(s => s.StreamReadLine()).Returns(() => "POST /serverInfoUpdate");
            stream.Setup(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(136);

            var getHandlerInvokingCount = 0;
            var getHandler = new Action<string>(s => getHandlerInvokingCount++);

            var postHandlerInvokingCount = 0;
            var postHandler = new Action<string, IMemoryStreamWrapper, IStreamWriterWrapper>((s, m, sw) => postHandlerInvokingCount++);

            var httpProc = new HttpProcessor(tcpClient.Object, streamFactory.Object, getHandler, postHandler);

            // Act.
            httpProc.Process();

            // Assert.
            streamWriter.Verify(s => s.Write(It.IsAny<string>()), Times.Exactly(3));
            Assert.AreEqual(0, getHandlerInvokingCount);
            Assert.AreEqual(0, postHandlerInvokingCount);
        }

        private static string GetStreamReadLineResponse(int callCount)
        {
            var responses = new[]
            {
                "POST /serverInfoUpdate HTTP/1.1",
                "User-Agent: UnityPlayer/5.2.0f3 (http://unity3d.com)",
                "Host: 127.0.0.1:58080",
                "Accept: */*",
                "Accept-Encoding: identity",
                "Content-Length: 136",
                "X-Unity-Version: 5.2.0f3",
                "Content-Type: application/x-www-form-urlencoded",
                ""
            };

            return responses[callCount];
        }
    }
}