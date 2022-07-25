using System.Net.Sockets;

namespace WebOverlay.Http
{
    internal abstract class AProtocl
    {
        protected readonly Client m_Client;
        protected readonly Socket m_Socket;

        protected AProtocl(Client client, Socket socket)
        {
            m_Client = client;
            m_Socket = socket;
        }

        internal abstract void ReadBuffer(byte[] buffer);
        internal abstract void WriteBuffer(string buffer);
    }
}
