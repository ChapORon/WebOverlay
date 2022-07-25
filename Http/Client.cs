using System.Net.Sockets;
using System.Text;

namespace WebOverlay.Http
{
    internal class Client
    {
        private readonly int m_ID;
        private readonly Socket m_Socket;
        private byte[] m_Buffer = new byte[1024];
        private readonly Server m_Server;
        private Resource? m_Resource = null;
        private AProtocl m_ReaderWriter;
        private bool m_IsWebSocket = false;

        internal Client(Server server, Socket socket, int id)
        {
            m_Server = server;
            m_Socket = socket;
            m_ID = id;
            m_ReaderWriter = new Protocol(this, socket);
        }

        internal void EnableWebSocket()
        {
            m_ReaderWriter = new WebSocket.Protocol(this, m_Socket);
            m_IsWebSocket = true;
        }

        internal bool IsWebSocket() { return m_IsWebSocket; }

        internal void ListenTo(Resource resource)
        {
            m_Resource?.RemoveListener(m_ID);
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

        internal void TreatRequest(Request httpRequest)
        {
            m_Server.TreatRequest(m_ID, httpRequest);
        }

        internal void TreatWebSocketMessage(string message)
        {
            m_Resource?.OnWebsocketMessage(m_ID, message);
        }

        internal void TreatWebSocketClose(short code, string message)
        {
            m_Resource?.RemoveListener(m_ID);
            MessageBox.Show(string.Format("[{0}]: {1}", code, message));
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int bytesRead = m_Socket.EndReceive(AR);
                if (bytesRead > 1)
                {
                    Array.Resize(ref m_Buffer, bytesRead);
                    m_ReaderWriter.ReadBuffer(m_Buffer);
                    StartReceiving();
                }
                else
                    Disconnect();
            }
            catch
            {
                if (!m_Socket.Connected)
                    Disconnect();
                else
                    StartReceiving();
            }
        }

        internal void Disconnect()
        {
            m_Resource?.RemoveListener(m_ID);
            m_Server.RemoveUser(m_ID);
            m_Socket.Disconnect(true);
        }

        internal void Send(string response)
        {
            m_ReaderWriter.WriteBuffer(response);
        }

        internal void SendResponse(Response? response)
        {
            if (response != null)
                Send(response.ToString());
        }
    }
}
