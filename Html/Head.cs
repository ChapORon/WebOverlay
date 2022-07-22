using System.Text;

namespace WebOverlay.Html
{
    public class Head : BaseElement
    {
        private readonly Dictionary<Meta.Type, Meta> m_MetaAttributes = new();
        private Base? m_Base = null;

        public Head(string title) : base("head")
        {
            AddElement(new Title(title));
        }

        public void SetCharset(string charset)
        {
            AddMeta(Meta.Type.CHARSET, charset);
        }

        public void AddMeta(Meta.Type type, string content)
        {
            if (!m_MetaAttributes.ContainsKey(type))
            {
                Meta meta = new(type);
                AddElement(meta);
                m_MetaAttributes.Add(type, meta);
            }
            m_MetaAttributes[type].SetContent(content);
        }

        public void SetBase(string href = "", Base.Type target = Base.Type.NONE)
        {
            if (m_Base == null)
            {
                m_Base = new();
                AddElement(m_Base);
            }
            m_Base.SetBase(href, target);
        }

        public void AddLink(Link link)
        {
            AddElement(link);
        }

        public void AddScript(Script script)
        {
            AddElement(script);
        }
    }
}
