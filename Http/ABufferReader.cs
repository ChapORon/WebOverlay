using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOverlay.Http
{
    internal abstract class ABufferReader<T>
    {
        private readonly List<T> m_Results = new();

        protected void AddResult(T result) { m_Results.Add(result); }

        internal void ClearResult() { m_Results.Clear(); }
        internal bool HaveResult() { return m_Results.Count > 0; }
        internal List<T> GetResult() { return m_Results; }

        internal abstract void ReadBuffer(byte[] buffer);
    }
}
