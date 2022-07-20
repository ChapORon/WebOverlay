using System.Text;

namespace WebOverlay.Html
{
    public class Head : BaseElement
    {
        public enum MetaType
        {
            APPLICATION_NAME,
            AUTHOR,
            CONTENT_SECURITY_POLICY,
            CONTENT_TYPE,
            DEFAULT_STYLE,
            DESCRIPTION,
            GENERATOR,
            KEYWORDS,
            REFRESH,
            VIEWPORT
        }

        public enum BaseType
        {
            BLANK,
            PARENT,
            SELF,
            TOP,
            NONE
        }

        private readonly string m_Title;
        private string m_Charset = "";
        private string m_BaseRef = "";
        private BaseType m_BaseType = BaseType.NONE;
        private readonly Dictionary<MetaType, string> m_MetaAttributes = new();
        private readonly List<Link> m_Links = new();
        private readonly List<Script> m_Scripts = new();

        public Head(string title) : base("head")
        {
            m_Title = title;
        }

        protected override bool IsEmpty()
        {
            return base.IsEmpty() &&
                string.IsNullOrWhiteSpace(m_Title) &&
                string.IsNullOrWhiteSpace(m_Charset) &&
                string.IsNullOrWhiteSpace(m_BaseRef) &&
                m_BaseType == BaseType.NONE &&
                m_MetaAttributes.Count == 0 &&
                m_Links.Count == 0 &&
                m_Scripts.Count == 0;
        }

        public void SetCharset(string charset)
        {
            m_Charset = charset;
        }

        public void SetBase(string href)
        {
            m_BaseRef = href;
            m_BaseType = BaseType.NONE;
        }

        public void SetBase(BaseType target)
        {
            m_BaseRef = "";
            m_BaseType = target;
        }

        public void SetBase(string href, BaseType target)
        {
            m_BaseRef = href;
            m_BaseType = target;
        }

        public void AddMeta(MetaType type, string content)
        {
            m_MetaAttributes[type] = content;
        }

        public void AddLink(Link link)
        {
            m_Links.Add(link);
        }

        public void AddScript(Script script)
        {
            m_Scripts.Add(script);
        }

        private void AppendTitle(ref StringBuilder builder)
        {
            if (!string.IsNullOrWhiteSpace(m_Title))
            {
                builder.Append("<title>");
                builder.Append(m_Title);
                builder.Append("</title>");
            }
        }

        private void AppendMeta(ref StringBuilder builder)
        {
            if (!string.IsNullOrWhiteSpace(m_Charset))
            {

                builder.Append("<meta charset=\"");
                builder.Append(m_Charset);
                builder.Append("\">");
            }
            foreach (var meta in m_MetaAttributes)
            {
                builder.Append("<meta ");
                switch (meta.Key)
                {
                    case MetaType.APPLICATION_NAME:
                        {
                            builder.Append("name=\"application-name");
                            break;
                        }
                    case MetaType.AUTHOR:
                        {
                            builder.Append("name=\"author");
                            break;
                        }
                    case MetaType.DESCRIPTION:
                        {
                            builder.Append("name=\"description");
                            break;
                        }
                    case MetaType.GENERATOR:
                        {
                            builder.Append("name=\"generator");
                            break;
                        }
                    case MetaType.KEYWORDS:
                        {
                            builder.Append("name=\"keywords");
                            break;
                        }
                    case MetaType.VIEWPORT:
                        {
                            builder.Append("name=\"viewport");
                            break;
                        }
                    case MetaType.CONTENT_SECURITY_POLICY:
                        {
                            builder.Append("http-equiv=\"content-security-policy");
                            break;
                        }
                    case MetaType.CONTENT_TYPE:
                        {
                            builder.Append("http-equiv=\"content-type");
                            break;
                        }
                    case MetaType.DEFAULT_STYLE:
                        {
                            builder.Append("http-equiv=\"default-style");
                            break;
                        }
                    case MetaType.REFRESH:
                        {
                            builder.Append("http-equiv=\"refresh");
                            break;
                        }
                }
                builder.Append("\" content=\"");
                builder.Append(meta.Value);
                builder.Append("\">");
            }
        }

        private void AppendLinks(ref StringBuilder builder)
        {
            foreach (Link link in m_Links)
                link.Append(ref builder);
        }

        private void AppendScripts(ref StringBuilder builder)
        {
            foreach (Script script in m_Scripts)
                script.Append(ref builder);
        }

        private void AppendBase(ref StringBuilder builder)
        {
            if (m_BaseType != BaseType.NONE || !string.IsNullOrWhiteSpace(m_BaseRef))
            {
                builder.Append("<base href=\"");
                if (!string.IsNullOrWhiteSpace(m_BaseRef))
                {
                    builder.Append(" href=\"");
                    builder.Append(m_BaseRef);
                    builder.Append('"');
                }
                switch (m_BaseType)
                {
                    case BaseType.BLANK:
                        {
                            builder.Append(" target=\"_blank\"");
                            break;
                        }
                    case BaseType.PARENT:
                        {
                            builder.Append(" target=\"_parent\"");
                            break;
                        }
                    case BaseType.SELF:
                        {
                            builder.Append(" target=\"_self\"");
                            break;
                        }
                    case BaseType.TOP:
                        {
                            builder.Append(" target=\"_top\"");
                            break;
                        }
                }
                builder.Append('>');
            }
        }

        public override void Append(ref StringBuilder builder)
        {
            AppendHeadTag(ref builder);
            AppendTitle(ref builder);
            AppendMeta(ref builder);
            AppendLinks(ref builder);
            AppendScripts(ref builder);
            AppendBase(ref builder);
            AppendTailTag(ref builder);
        }
    }
}
