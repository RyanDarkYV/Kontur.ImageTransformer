using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amib.Threading;
using Kontur.ImageTransformer.Handlers.RequestHandlers;

namespace Kontur.ImageTransformer.Server
{
    internal class AsyncHttpServer : IDisposable
    {
        #region Private Fields
        private readonly HttpListener _listener;
        private Thread _listenerThread;
        private bool _disposed;
        private volatile bool _isRunning;
        private ServerConfig _serverConfig;
        private SmartThreadPool _threadPool;
        private Semaphore _semaphore;
        #endregion

        #region AsyncHttpServer constructors
        public AsyncHttpServer() : this(new ServerConfig())
        {
            _listener = new HttpListener();
        }

        public AsyncHttpServer(ServerConfig serverConfig)
        {
            _listener = new HttpListener();
            _threadPool = new SmartThreadPool(){ MaxThreads =  serverConfig.MaxThreads};
            RequestHandler.MaxProcessingTime = serverConfig.Timeout;
            _semaphore = new Semaphore(serverConfig.CountIO, serverConfig.CountIO);
            _serverConfig = serverConfig;

        }
        #endregion

        #region StartServer method
        public void StartServer(string prefix)
        {
            lock (_listener)
            {
                if (!_isRunning)
                {
                    _listener.Prefixes.Clear();
                    _listener.Prefixes.Add(prefix);
                    _listener.Start();

                    _listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };

                    _listenerThread.Start();
                    _isRunning = true;
                }
            }
        }
        #endregion

        #region Listen method
        private void Listen()
        {
            while (true)
            {
                _semaphore.WaitOne();
                ListenForRequest();
                try
                {
                    if (_listener.IsListening)
                    {
                        var context = _listener.GetContext();
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception e)
                {
                    // TODO: Log errors
                }
            }
        }

        private void ListenForRequest()
        {
            var newContext = _listener.BeginGetContext(Callback, _listener);
        }

        private void Callback(IAsyncResult result)
        {
            var contextQueue = new ContextQueue();
            contextQueue.StartTimer();
            var listener = (HttpListener) result.AsyncState;
            var context = listener.EndGetContext(result);
            _semaphore.Release();

            contextQueue.SetContext(context);
#pragma warning disable 4014
            HandleContextAsync(contextQueue.Context);
#pragma warning restore 4014
        }
        #endregion

        #region HandleContext method
        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            if (!IsThreadPoolAvailable())
            {
                try
                {
                    if (listenerContext.Request.HttpMethod != "POST")
                    {
                        listenerContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        RequestHandler.CloseResponseWithCode(listenerContext,HttpStatusCode.BadRequest);
                        return;
                    }

                    _threadPool.QueueWorkItem(RequestHandler.HandleRequest, listenerContext);
                }
                catch
                {
                    RequestHandler.CloseResponseWithCode(listenerContext, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                RequestHandler.CloseResponseWithCode(listenerContext, HttpStatusCode.BadRequest);
            }
        }
        #endregion
        
        #region StopServer method
        public void Stop()
        {
            lock (_listener)
            {
                if (!_isRunning)
                    return;

                _listener.Stop();
                _listenerThread.Abort();
                _listenerThread.Join();

                _isRunning = false;

            }
        }
        #endregion

        #region Dispose method
        public void Dispose()
        {
            if (_disposed) 
                return;

            _disposed = true;
            Stop();
            _listener.Close();
        }
        #endregion

        #region Threads
        private bool IsThreadPoolAvailable()
        {
            var itemsInWork = _threadPool.CurrentWorkItemsCount;
            var avg = RequestHandler.RequestProcessedCount;

            return itemsInWork * avg > _serverConfig.Timeout;
        }
        #endregion
        
    }
}