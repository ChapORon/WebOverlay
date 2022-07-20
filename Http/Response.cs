using System.Text;

namespace WebOverlay.Http
{
    public class Response
    {
        private readonly string m_Version;
        private readonly int m_StatusCode;
        private readonly string m_StatusMessage;
        private readonly HeaderFields m_HeaderFields = new();
        private readonly string m_Body;

        public Response(string version, int statusCode, string statusMessage, string body = "")
        {
            m_Version = version;
            m_StatusCode = statusCode;
            m_StatusMessage = statusMessage;
            m_Body = body;
            if (m_Body.Length > 0)
            {
                m_HeaderFields["Accept-Ranges"] = "bytes";
                m_HeaderFields["Content-Length"] = Encoding.Default.GetBytes(m_Body).Length;
            }
            m_HeaderFields["Server"] = "Web Overlay HTTP Server";
            m_HeaderFields["Content-Type"] = "text/html";
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append(m_Version);
            builder.Append(' ');
            builder.Append(m_StatusCode);
            builder.Append(' ');
            builder.Append(m_StatusMessage);
            builder.Append(Defines.CRLF);
            builder.Append(m_HeaderFields);
            builder.Append(m_Body);
            return builder.ToString();
        }

        public bool HaveHeaderField(string headerFieldName) { return m_HeaderFields.HaveHeaderField(headerFieldName); }

        public string Version { get => m_Version; }
        public int StatusCode { get => m_StatusCode; }
        public string StatusMessage { get => m_StatusMessage; }
        public string Body { get => m_Body; }
        public object this[string key]
        {
            get => m_HeaderFields[key];
            set => m_HeaderFields[key] = value;
        }
    }
}
