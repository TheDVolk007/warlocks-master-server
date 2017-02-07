using System;
using System.Collections;
using System.IO;
using App.Common.Wrappers;


// ReSharper disable InconsistentNaming
// ReSharper disable LocalizableElement


// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske. 

// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

namespace App.HttpServerScripts.Implementations
{
    public class HttpProcessor : IHttpProcessor
    {
        private readonly ITcpClientWrapper tcpClient;
        private readonly IStreamFactory streamFactory;
        private readonly Action<string> getHandler;
        private readonly Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> postHandler;

        private string httpMethod;
        private string httpUrl;
        private readonly Hashtable httpHeaders = new Hashtable();
        
        private const int MaxPostSize = 1 * 1024; // 1 kB
        private const int BufSize = 4096;

        public HttpProcessor(ITcpClientWrapper tcpClient,
            IStreamFactory streamFactory,
            Action<string> getHandler,
            Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> postHandler)
        {
            this.tcpClient = tcpClient;
            this.streamFactory = streamFactory;
            this.getHandler = getHandler;
            this.postHandler = postHandler;
        }
        
        public void Process()
        {
            using (var inputStream = tcpClient.GetStream())
            using(var outputStream = streamFactory.GetStreamWriterWrapper(inputStream))
            {
                try
                {
                    ParseRequest(inputStream);
                    ReadHeaders(inputStream);
                    if(httpMethod.Equals("GET"))
                    {
                        HandleGETRequest();
                    }
                    else if(httpMethod.Equals("POST"))
                    {
                        HandlePOSTRequest(inputStream, outputStream);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    WriteFailure(outputStream);
                }
            }
            tcpClient.Close();
        }

        private void ParseRequest(IStreamWrapper inputStream)
        {
            var request = inputStream.StreamReadLine();
            var tokens = request.Split(' ');
            if(tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            httpMethod = tokens[0].ToUpper();
            httpUrl = tokens[1];

            Console.WriteLine("starting: " + request);
        }

        private void ReadHeaders(IStreamWrapper inputStream)
        {
            Console.WriteLine("readHeaders()");
            string line;
            while((line = inputStream.StreamReadLine()) != null)
            {
                if(line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                var separator = line.IndexOf(':');
                if(separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }

                var name = line.Substring(0, separator);
                var pos = separator + 1;
                while((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                var value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }
        
        private void HandleGETRequest()
        {
            getHandler(httpUrl);
        }

        private void HandlePOSTRequest(IStreamWrapper inputStream, IStreamWriterWrapper outputStream)
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            //Console.WriteLine("get post data start");

            using(var ms = streamFactory.GetMemoryStreamWrapper())
            {
                if(httpHeaders.ContainsKey("Content-Length"))
                {
                    var contentLen = Convert.ToInt32(httpHeaders["Content-Length"]);
                    if(contentLen > MaxPostSize)
                    {
                        throw new Exception($"POST Content-Length({contentLen}) too big for this server");
                    }
                    var buf = new byte[BufSize];
                    var toRead = contentLen;
                    while(toRead > 0)
                    {
                        //Console.WriteLine("starting Read, to_read={0}", toRead);
                        var numread = inputStream.Read(buf, 0, Math.Min(BufSize, toRead));

                        //Console.WriteLine("read finished, numread={0}", numread);
                        if(numread == 0)
                        {
                            if(toRead == 0)
                            {
                                break;
                            }

                            throw new Exception("client disconnected during post");
                        }
                        toRead -= numread;
                        ms.Write(buf, 0, numread);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                }
                //Console.WriteLine("get post data end");

                postHandler(httpUrl, ms, outputStream);
            }
        }

        //public void WriteSuccess(StreamWriter outputStream)
        //{
        //    outputStream.Write("HTTP/1.0 200 OK\n");
        //    outputStream.Write("Content-Type: text/html\n");
        //    outputStream.Write("Connection: close\n");
        //    outputStream.Write("\n");
        //}

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WriteFailure(IStreamWriterWrapper outputStream)
        {
            outputStream.Write("HTTP/1.0 404 File not found\n");
            outputStream.Write("Connection: close\n");
            outputStream.Write("\n");
        }
    }
}