using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace HttpServer
{
    public abstract class HttpServer : IServer, IDisposable
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp { get; private set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// 是否运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 服务器协议
        /// </summary>
        public Protocols Protocol { get; private set; }

        /// <summary>
        /// 服务端Socet
        /// </summary>
        private TcpListener _serverListener;

        /// <summary>
        /// 日志接口
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// SSL证书
        /// </summary>
        private X509Certificate _serverCertificate = null;

        private readonly static object Lock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        private HttpServer(IPAddress ipAddress, int port)
        {
            this.ServerIp = ipAddress.ToString();
            this.ServerPort = port;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public HttpServer(string ipAddress, int port) :
            this(IPAddress.Parse(ipAddress), port)
        { }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">端口号</param>
        public HttpServer(int port) :
            this(IPAddress.Loopback, port)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        public HttpServer(string ip) :
            this(IPAddress.Parse(ip), 80)
        { }

        #region 公开方法

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;

            //创建服务端Socket
            this._serverListener = new TcpListener(IPAddress.Parse(ServerIp), ServerPort);
            this.Protocol = _serverCertificate == null ? Protocols.Http : Protocols.Https;
            this.IsRunning = true;
            this._serverListener.Start();
            this.Log($"Sever is running at {Protocol.ToString().ToLower()}://{ServerIp}:{ServerPort}");

            try
            {
                while (IsRunning)
                {
                    //监听请求的数据
                    var client = _serverListener.AcceptTcpClient();
                    var requestThread = new Thread(() => { ProcessRequest(client); });
                    requestThread.Start();
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }

            this.IsRunning = false;
        }


        public HttpServer SetSsl(string certificate)
        {
            return SetSsl(X509Certificate.CreateFromCertFile(certificate));
        }


        public HttpServer SetSsl(X509Certificate certifiate)
        {
            this._serverCertificate = certifiate;
            return this;
        }

        public void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
            _serverListener.Stop();
        }

        /// <summary>
        /// 设置端口
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public HttpServer SetPort(int port)
        {
            this.ServerPort = port;
            return this;
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 处理客户端请求
        /// </summary>
        /// <param name="handler">客户端Socket</param>
        private void ProcessRequest(TcpClient handler)
        {
            //处理请求
            Stream clientStream = handler.GetStream();

            //处理SSL
            if (_serverCertificate != null)
                clientStream = ProcessSsl(clientStream);

            if (clientStream == null) return;

            //构造HTTP请求
            var request = new HttpRequest(clientStream) { Logger = Logger };

            //构造HTTP响应
            var response = new HttpResponse(clientStream) { Logger = Logger };

            lock (Lock)
            {
                //处理请求类型
                switch (request.Method)
                {
                    case "GET":
                        OnGet(request, response);
                        break;
                    case "POST":
                        OnPost(request, response);
                        break;
                    default:
                        OnDefault(request, response);
                        break;
                }
            }
        }


        /// <summary>
        /// 处理ssl加密请求
        /// </summary>
        /// <param name="clientStream"></param>
        /// <returns></returns>
        private Stream ProcessSsl(Stream clientStream)
        {
            try
            {
                var sslStream = new SslStream(clientStream);
                sslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls, true);
                sslStream.ReadTimeout = 10000;
                sslStream.WriteTimeout = 10000;
                return sslStream;
            }
            catch (Exception e)
            {
                Log(e.Message);
                clientStream.Close();
            }

            return null;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message">日志消息</param>
        protected void Log(object message)
        {
            Logger?.Log(message);
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 响应Get请求
        /// </summary>
        /// <param name="request">请求报文</param>
        /// <param name="response"></param>
        public virtual void OnGet(HttpRequest request, HttpResponse response)
        {

        }

        /// <summary>
        /// 响应Post请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void OnPost(HttpRequest request, HttpResponse response)
        {

        }

        /// <summary>
        /// 响应默认请求
        /// </summary>

        public virtual void OnDefault(HttpRequest request, HttpResponse response)
        {

        }

        public abstract void Dispose();

        #endregion
    }
}
