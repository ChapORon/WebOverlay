using System.Net;
using System.Net.Sockets;

namespace WebOverlay.Http
{
    public class Server
    {
        private readonly Socket m_WebServerSocket;
        private readonly int m_Port;
        private readonly IDGenerator m_IDGenerator = new();
        private readonly Dictionary<int, Client> m_Clients = new();
        private readonly Dictionary<string, Resource> m_Resources = new();

        public Server(int port)
        {
            m_Port = port;
            m_WebServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void AddResource<TRes>(string url, params dynamic[] args) where TRes : Resource, new()
        {
            Resource newResource = new TRes();
            newResource.Init(this, url, args);
            m_Resources[url] = newResource;
        }

        public void StartListening()
        {
            try
            {
                m_WebServerSocket.Bind(new IPEndPoint(IPAddress.Any, m_Port));
                m_WebServerSocket.Listen(10);
                m_WebServerSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                throw new Exception("listening error" + ex);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine($"Accept CallBack port:{m_Port} protocol type: {ProtocolType.Tcp}");
                Socket acceptedSocket = m_WebServerSocket.EndAccept(ar);
                Client client = new(this, acceptedSocket, m_IDGenerator.NextIdx());
                m_Clients[client.GetID()] = client;
                client.StartReceiving();
                m_WebServerSocket.BeginAccept(AcceptCallback, m_WebServerSocket);
            }
            catch (Exception ex)
            {
                throw new Exception("Base Accept error" + ex);
            }
        }

        internal void RemoveUser(int clientID)
        {
            m_Clients.Remove(clientID);
            m_IDGenerator.FreeIdx(clientID);
        }

        internal void TreatRequest(int clientID, Request request)
        {
            if (m_Resources.TryGetValue(request.Path, out var resource))
                resource.OnRequest(clientID, request);
            else
                SendError(clientID, Defines.NewResponse(request.Version, 404));
        }

        public void SendResponse(int clientID, string response)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
                client.SendResponse(response);
        }

        public void SendError(int clientID, Response? response)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
            {
                client.SendResponse(response);
                client.Disconnect();
            }
        }

        public void SendResponse(int clientID, Response? response)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
                client.SendResponse(response);
        }

        internal void SetClientType(int clientID, ClientType clientType)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
                client.SetClientType(clientType);
        }

        internal ClientType GetClientType(int clientID)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
                return client.GetClientType();
            return ClientType.Invalid;
        }

        internal void ListenTo(int clientID, Resource resource)
        {
            if (m_Clients.TryGetValue(clientID, out var client))
                client.ListenTo(resource);
        }
    }
}
