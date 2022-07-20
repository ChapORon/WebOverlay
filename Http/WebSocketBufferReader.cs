using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOverlay.Http
{
    internal class WebSocketBufferReader: ABufferReader<string>
    {
        internal override void ReadBuffer(byte[] buffer)
        {
            bool fin = (buffer[0] & 0b10000000) != 0;
            bool mask = (buffer[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
            int opcode = buffer[0] & 0b00001111; // expecting 1 - text message
            int offset = 2;
            ulong msglen = (ulong)(buffer[1] & 0b01111111);

            if (msglen == 126)
            {
                // bytes are reversed because websocket will print them in Big-Endian, whereas
                // BitConverter will want them arranged in little-endian on windows
                msglen = BitConverter.ToUInt16(new byte[] { buffer[3], buffer[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering 
                // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                // websocket frame available through client.Available).  
                msglen = BitConverter.ToUInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                offset = 10;
            }

            if (msglen != 0 && mask)
            {
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3] };
                offset += 4;

                for (ulong i = 0; i < msglen; ++i)
                    decoded[i] = (byte)(buffer[(ulong)offset + i] ^ masks[i % 4]);
                AddResult(Encoding.Default.GetString(decoded));
            }
        }
    }
}
