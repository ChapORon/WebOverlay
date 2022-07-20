using System.Text;

namespace WebOverlay.Html
{
    public class Document : BaseElement
    {
        private readonly Head m_Head;
        private readonly Element m_Body = new("body");

        public Document(string title) : base("html")
        {
            m_Head = new(title);
            AddElement(m_Head);
            AddElement(m_Body);
        }

        public Head GetHead() { return m_Head; }
        public Element GetBody() { return m_Body; }

        public override void Append(ref StringBuilder builder)
        {
            builder.Append("<!DOCTYPE html>");
            AppendHeadTag(ref builder);
            builder.Append(m_Head.ToString());
            builder.Append(m_Body.ToString());
            AppendTailTag(ref builder);
        }
    }
}
