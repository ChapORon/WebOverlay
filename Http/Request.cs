namespace WebOverlay.Http
{
    public class Request
    {
        private string m_Method;
        private string m_Path;
        private string m_Version;
        private readonly HeaderFields m_HeaderFields; //All header fields value will be stored as string in request
        private readonly string m_Body = "";

        public Request(string request)
        {
            List<string> attributes = request.Split(new string[] { Defines.CRLF }, StringSplitOptions.None).ToList();
            string httpRequest = attributes[0];
            attributes.RemoveAt(0);
            m_HeaderFields = new(attributes);
            //Handle the HTTP version
            int idx = httpRequest.LastIndexOf("HTTP");
            m_Version = httpRequest[idx..];
            httpRequest = httpRequest[..idx];
            //Handle the HTTP method
            idx = httpRequest.IndexOf(" ");
            m_Method = httpRequest[..idx];
            httpRequest = httpRequest[idx..];
            //Handle the HTTP Path
            m_Path = httpRequest.Trim();
        }

        public Request(Request request, string body)
        {
            m_Method = request.Method;
            m_Path = request.Path;
            m_Version = request.Version;
            m_HeaderFields = new(request.m_HeaderFields);
            m_Body = body;
        }

        public bool HaveHeaderField(string headerFieldName) { return m_HeaderFields.HaveHeaderField(headerFieldName); }

        public string Method { get => m_Method; }
        public string Path { get => m_Path; }
        public string Version { get => m_Version; }
        public string Body { get => m_Body; }
        public object this[string key]
        {
            get => m_HeaderFields[key];
        }
    }
}
