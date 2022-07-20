using System.Text;

namespace WebOverlay.Http
{
    internal class RequestBufferReader: ABufferReader<Request>
    {
        private string m_Buffer = "";

        internal override void ReadBuffer(byte[] buffer)
        {
            m_Buffer += Encoding.Default.GetString(buffer, 0, buffer.Length);
            int position;
            do
            {
                position = m_Buffer.IndexOf(Defines.CRLF + Defines.CRLF);
                if (position >= 0)
                {
                    string data = m_Buffer[..position];
                    m_Buffer = m_Buffer[(position + (Defines.CRLF.Length * 2))..];
                    Request httpRequest = new(data);
                    //TODO Handle request body
                    AddResult(httpRequest);
                }
            } while (position >= 0);
        }
    }
}
