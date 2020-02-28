using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpServer
{
    public class HttpResponse : BaseHeader
    {
        public string StatusCode { get; set; }

        public string Protocols { get; set; }

        public string ProtocolsVersion { get; set; }

        public byte[] Content { get; private set; }

        private readonly Stream _handler;

        public ILogger Logger { get; set; }

        public HttpResponse(Stream stream)
        {
            this._handler = stream;
            this.Headers = new Dictionary<string, string>();
        }

        public HttpResponse SetContent(byte[] content, Encoding encoding = null)
        {
            this.Content = content;
            this.Encoding = encoding ?? Encoding.UTF8;
            this.Content_Length = content.Length.ToString();
            return this;
        }

        public HttpResponse SetContent(string content, Encoding encoding = null)
        {
            //初始化内容
            encoding = encoding ?? Encoding.UTF8;
            return SetContent(encoding.GetBytes(content), encoding);
        }

        public Stream GetResponseStream()
        {
            return this._handler;
        }

        public string GetHeader(ResponseHeaders header)
        {
            return GetHeaderByKey(header);
        }

        public string GetHeader(string fieldName)
        {
            return GetHeaderByKey(fieldName);
        }

        public void SetHeader(ResponseHeaders header, string value)
        {
            SetHeaderByKey(header, value);
        }

        public void SetHeader(string fieldName, string value)
        {
            SetHeaderByKey(fieldName, value);
        }

        /// <summary>
        /// 构建响应头部
        /// </summary>
        /// <returns></returns>
        protected string BuildHeader()
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(StatusCode))
                builder.Append("HTTP/1.1 " + StatusCode + "\r\n");

            if (!string.IsNullOrEmpty(this.Content_Type))
                builder.AppendLine("Content-Type:" + this.Content_Type);
            return builder.ToString();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send()
        {
            if (!_handler.CanWrite) return;

            try
            {
                //发送响应头
                var header = BuildHeader();
                var headerBytes = this.Encoding.GetBytes(header);
                _handler.Write(headerBytes, 0, headerBytes.Length);

                //发送空行
                var lineBytes = this.Encoding.GetBytes(System.Environment.NewLine);
                _handler.Write(lineBytes, 0, lineBytes.Length);

                //发送内容
                _handler.Write(Content, 0, Content.Length);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
            finally
            {
                _handler.Close();
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message">日志消息</param>
        private void Log(object message)
        {
            Logger?.Log(message);
        }
    }
}
