using System.Net.Sockets;
using System.Text;

namespace WebOverlay.Http.WebSocket
{
    internal class Protocol : AProtocl
    {
        private readonly StringBuilder m_ReadBuffer = new();

        public Protocol(Client client, Socket socket) : base(client, socket) {}

        internal override void ReadBuffer(byte[] buffer)
        {
            Frame frame = new(buffer);
            m_ReadBuffer.Append(frame.GetContent());
            if (frame.IsFin())
            {
                if (frame.GetOpCode() == 1) //1 - text message
                    m_Client.TreatWebSocketMessage(m_ReadBuffer.ToString());
                else if (frame.GetOpCode() == 8) //8 - close message
                    m_Client.TreatWebSocketClose(frame.GetStatusCode(), m_ReadBuffer.ToString());
                else if (frame.GetOpCode() == 9) //9 - ping message
                    WriteToWebsocket(new(true, 10, m_ReadBuffer.ToString())); //10 - pong message
                m_ReadBuffer.Clear();
            }
        }

        private void WriteToWebsocket(Frame frame)
        {
            byte[] response = frame.GetBytes();
            m_Socket.Send(response, response.Length, 0);
        }

        internal override void WriteBuffer(string buffer)
        {
            WriteToWebsocket(new(true, 1, buffer));
        }
    }
}
