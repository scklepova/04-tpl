using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HashServer;
using log4net;
using log4net.Config;

namespace Proxy
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			try
			{
                InitListOfServerNames();

				var listener = new Listener(port, "method", OnContextAsync);
				listener.Start();

				log.InfoFormat("Server started!");
				new ManualResetEvent(false).WaitOne();
			}
			catch (Exception e)
			{
				log.Fatal(e);
				throw;
			}
		}

		private static async Task OnContextAsync(HttpListenerContext context)
		{
			var requestId = Guid.NewGuid();
			var query = context.Request.QueryString["query"];
			var remoteEndPoint = context.Request.RemoteEndPoint;
			log.InfoFormat("{0}: received {1} from {2}", requestId, query, remoteEndPoint);
			context.Request.InputStream.Close();

            ShuffleServers();
		    var page = await GetResponseAsync(query);
		    if (page != null)
		    {
		        if (SupportDeflateCompressing(context))
		        {
		            context.Response.AddHeader("Content-Encoding", "deflate");
		            await WriteDeflateCompressedAsync(page, context.Response.OutputStream);
                    log.InfoFormat("deflate");
		        }
		        else
		        {
		            await context.Response.OutputStream.WriteAsync(page, 0, page.Length);
		        }
                log.InfoFormat("{0}: {1} sent back to {2}", requestId, 0, remoteEndPoint);
		    }
		    else
		    {
		        context.Response.StatusCode = 500;
                log.InfoFormat("All servers failed");
		    }
		}


	    private static bool SupportDeflateCompressing(HttpListenerContext context)
	    {
	        return context.Request.Headers["Accept-Encoding"] != null && context.Request.Headers["Accept-Encoding"].Contains("deflate");
	    }


	    private static async Task WriteDeflateCompressedAsync(byte[] page, Stream stream)
        {
            using (var originalStream = new MemoryStream(page)) 
            {
                using(var deflateStream = new DeflateStream(stream, CompressionMode.Compress)) 
        	    {
        	            await originalStream.CopyToAsync(deflateStream);          
        	    }
        	}
        }


	    private static async Task<byte[]> GetResponseAsync(string query)
	    {
	        const int timeout = 1000*5;
	        var serverNumber = -1;

	        while (NextServerExists(serverNumber))
	        {
	            var serverAddr = GetNextServer(serverNumber);
	            log.InfoFormat("Redirected to {0}", serverAddr);

	            var task1 = DownloadWebPageAsync(String.Format("http://{0}/method?query={1}", serverAddr, query));

	            var taskDelay = Task.Delay(timeout);
	            var finishedTask = await Task.WhenAny(task1, taskDelay);
	            if (finishedTask == task1 && !task1.IsFaulted)
	                return task1.Result;
	            serverNumber++;
	        }
	        return null;
	    }


		public static async Task<byte[]> DownloadWebPageAsync(string address)
		{
			var sw = Stopwatch.StartNew();
			var request = CreateRequest(address);
			var response = await request.GetResponseAsync();
            
			using (var stream = response.GetResponseStream())
			{
				var ms = new MemoryStream();
				await stream.CopyToAsync(ms);
				return ms.ToArray();
			}
		}

		private static HttpWebRequest CreateRequest(string uriStr, int timeout = 30 * 1000)
		{
			var request = WebRequest.CreateHttp(uriStr);
			request.Timeout = timeout;
			request.Proxy = null;
			request.KeepAlive = true;
			request.ServicePoint.UseNagleAlgorithm = false;
			request.ServicePoint.ConnectionLimit = 100000000;
			return request;
		}

	    private static void InitListOfServerNames()
	    {
	        using (var reader = new StreamReader("servers.txt"))
	        {
	            while(!reader.EndOfStream)
                    serverAdresses.Add(reader.ReadLine());
	        }
	    }

	    private static void ShuffleServers()
	    {
 	        var random = new Random();

            for (var i = 0; i < serverAdresses.Count; i++)
            {
                var temp = serverAdresses[i];
                var randomIndex = random.Next(i, serverAdresses.Count);
                serverAdresses[i] = serverAdresses[randomIndex];
                serverAdresses[randomIndex] = temp;
            }
            
	    }

	    private static string GetNextServer(int serverNumber)
	    {
	        serverNumber++;
	        return serverNumber < serverAdresses.Count ? serverAdresses[serverNumber] : null;
	    }

	    private static bool NextServerExists(int serverNumber)
	    {
	        return serverNumber < serverAdresses.Count - 1;
	    }

	    private const int port = 20000;
		private static readonly byte[] Key = Encoding.UTF8.GetBytes("Контур.Шпора");
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static readonly List<string> serverAdresses = new List<string>();
	}
}