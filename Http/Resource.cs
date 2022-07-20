﻿using System.Text;

namespace WebOverlay.Http
{
    public class Resource
    {
        private Server? m_Server = null;
        private string m_URL = "";
        private readonly List<int> m_Listeners = new();

        public void Init(Server server, string url, params dynamic[] args)
        {
            m_URL = url;
            m_Server = server;
            OnInit(args);
        }

        protected virtual void OnInit(params dynamic[] args) { }

        public string URL { get { return m_URL; } }

        protected void SendError(int userID, Response? response)
        {
            if (m_Server != null)
                m_Server.SendError(userID, response);
        }

        protected void SendResponse(int userID, Response? response)
        {
            if (m_Server != null)
                m_Server.SendResponse(userID, response);
        }

        protected void SendResponse(int userID, string response)
        {
            if (m_Server != null)
                m_Server.SendResponse(userID, response);
        }

        internal void AddListener(int listenerID)
        {
            m_Listeners.Add(listenerID);
        }

        internal void RemoveListener(int listenerID)
        {
            m_Listeners.Remove(listenerID);
        }

        internal virtual void OnRequest(int clientID, Request request)
        {
            if (request.HaveHeaderField("Sec-WebSocket-Key"))
            {
                Response response = new(request.Version, 101, "Switching Protocols");
                response["Server"] = "Web Overlay HTTP Server";
                response["Content-Type"] = "text/html";
                response["Connection"] = "Upgrade";
                response["Upgrade"] = "websocket";
                response["Sec-WebSocket-Accept"] = Convert.ToBase64String(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes((string)request["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));
                if (m_Server != null)
                {
                    m_Server.SetClientType(clientID, ClientType.WebSocket);
                    m_Server.ListenTo(clientID, this);
                }
                SendResponse(clientID, response);
                return;
            }
            if (m_Server != null && m_Server.GetClientType(clientID) == ClientType.None)
                m_Server.SetClientType(clientID, ClientType.Browser);
            switch (request.Method)
            {
                case "GET":
                    {
                        Get(clientID, request);
                        break;
                    }
                case "HEAD":
                    {
                        Head(clientID, request);
                        break;
                    }
                case "POST":
                    {
                        Post(clientID, request);
                        break;
                    }
                case "PUT":
                    {
                        Put(clientID, request);
                        break;
                    }
                case "DELETE":
                    {
                        Delete(clientID, request);
                        break;
                    }
                case "CONNECT":
                    {
                        Connect(clientID, request);
                        break;
                    }
                case "OPTIONS":
                    {
                        Options(clientID, request);
                        break;
                    }
                case "TRACE":
                    {
                        Trace(clientID, request);
                        break;
                    }
                case "PATCH":
                    {
                        Patch(clientID, request);
                        break;
                    }
            }
        }

        internal void OnWebsocketMessage(int clientID, string message)
        {
            WebsocketMessage(clientID, message);
        }

        protected virtual void WebsocketMessage(int clientID, string message) { }

        private void NotImplementedYet(int clientID, string version) { SendError(clientID, Defines.NewResponse(version, 501)); }

        protected virtual void Get(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Head(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Post(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Put(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Delete(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Connect(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Options(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Trace(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
        protected virtual void Patch(int clientID, Request request) { NotImplementedYet(clientID, request.Version); }
    }
}
