using System.Net.Sockets;
using System.Text;

namespace WebOverlay.Http
{
    internal enum ClientType
    {
        None,
        WebSocket,
        Browser,
        Invalid
    }

    internal class Client
    {
        private ClientType m_ClientType = ClientType.None;
        private readonly int m_ID;
        private readonly Socket m_Socket;
        private byte[] m_Buffer = new byte[1024];
        private readonly Server m_Server;
        private Resource? m_Resource = null;
        private readonly ABufferReader<Request> m_RequestBufferReader = new RequestBufferReader();
        private readonly ABufferReader<string> m_WebSocketBufferReader = new WebSocketBufferReader();

        internal Client(Server server, Socket socket, int id)
        {
            m_Server = server;
            m_Socket = socket;
            m_ID = id;
        }

        internal ClientType GetClientType() { return m_ClientType; }
        internal void SetClientType(ClientType clientType) { m_ClientType = clientType; }

        internal void ListenTo(Resource resource)
        {
            if (m_Resource != null)
                m_Resource.RemoveListener(m_ID);
            m_Resource = resource;
            m_Resource.AddListener(m_ID);
        }

        internal int GetID() { return m_ID; }

        internal void StartReceiving()
        {
            try
            {
                m_Buffer = new byte[1024];
                m_Socket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch { }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int bytesRead = m_Socket.EndReceive(AR);
                if (bytesRead > 1)
                {
                    Array.Resize(ref m_Buffer, bytesRead);
                    switch (m_ClientType)
                    {
                        case ClientType.None:
                        case ClientType.Browser:
                            {
                                m_RequestBufferReader.ReadBuffer(m_Buffer);
                                if (m_RequestBufferReader.HaveResult())
                                {
                                    foreach (Request httpRequest in m_RequestBufferReader.GetResult())
                                        m_Server.TreatRequest(m_ID, httpRequest);
                                    m_RequestBufferReader.ClearResult();
                                }
                                break;
                            }
                        case ClientType.WebSocket:
                            {
                                m_WebSocketBufferReader.ReadBuffer(m_Buffer);
                                if (m_WebSocketBufferReader.HaveResult())
                                {
                                    if (m_Resource != null)
                                    {
                                        foreach (string message in m_WebSocketBufferReader.GetResult())
                                            m_Resource.OnWebsocketMessage(m_ID, message);
                                    }
                                    m_WebSocketBufferReader.ClearResult();
                                }
                                break;
                            }
                    }
                    StartReceiving();
                }
                else
                {
                    Disconnect();
                }
            }
            catch
            {
                if (!m_Socket.Connected)
                {
                    Disconnect();
                }
                else
                {
                    StartReceiving();
                }
            }
        }

        internal void Disconnect()
        {
            if (m_Resource != null)
                m_Resource.RemoveListener(m_ID);
            m_Server.RemoveUser(m_ID);
            m_Socket.Disconnect(true);
        }

        internal void SendResponse(string response)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(response);
                if (m_Socket.Connected)
                    m_Socket.Send(data, data.Length, 0);
            }
            catch  {}
        }

        internal void SendResponse(Response? response)
        {
            if (response == null)
                return;
            SendResponse(response.ToString());
        }
    }
}
