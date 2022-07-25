using System.Net.Sockets;
using System.Text;

namespace WebOverlay.Http
{
    internal class Protocol : AProtocl
    {
        private string m_ReadBuffer = "";

        public Protocol(Client client, Socket socket) : base(client, socket) { }

        internal override void ReadBuffer(byte[] buffer)
        {
            m_ReadBuffer += Encoding.Default.GetString(buffer, 0, buffer.Length);
            int position;
            do
            {
                position = m_ReadBuffer.IndexOf(Defines.CRLF + Defines.CRLF);
                if (position >= 0)
                {
                    string data = m_ReadBuffer[..position];
                    m_ReadBuffer = m_ReadBuffer[(position + (Defines.CRLF.Length * 2))..];
                    Request httpRequest = new(data);
                    //TODO Handle request body
                    m_Client.TreatRequest(httpRequest);
                }
            } while (position >= 0);
        }

        internal override void WriteBuffer(string buffer)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(buffer);
                if (m_Socket.Connected)
                    m_Socket.Send(data, data.Length, 0);
            }
            catch {}
        }
    }
}
