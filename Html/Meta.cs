namespace WebOverlay.Html
{
    public class Meta : BaseElement
    {
        public enum Type
        {
            CHARSET,
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

        private readonly Type m_Type;

        public Meta(Type type) : base("meta", isEmptyTag: true)
        {
            m_Type = type;

            AddStrictValidationRule(new()
            {
                {"charset", new IsNonEmptyStringRule()}, //Specifies the character encoding for the HTML document
                {"content", new IsNonEmptyStringRule()}, //Specifies the value associated with the http-equiv or name attribute
                {"http-equiv", new IsInListRule<string>(new(){ "content-security-policy", "content-type", "default-style", "refresh"})}, //Provides an HTTP header for the information/value of the content attribute
                {"name", new IsInListRule<string>(new(){ "application-name", "author", "description", "generator", "keywords", "viewport"})} //Specifies a name for the metadata
            });

            _ = type switch
            {
                Type.APPLICATION_NAME => SetAttribute("name", "application-name"),
                Type.AUTHOR => SetAttribute("name", "author"),
                Type.DESCRIPTION => SetAttribute("name", "description"),
                Type.GENERATOR => SetAttribute("name", "generator"),
                Type.KEYWORDS => SetAttribute("name", "keywords"),
                Type.VIEWPORT => SetAttribute("name", "viewport"),
                Type.CONTENT_SECURITY_POLICY => SetAttribute("http-equiv", "content-security-policy"),
                Type.CONTENT_TYPE => SetAttribute("http-equiv", "content-type"),
                Type.DEFAULT_STYLE => SetAttribute("http-equiv", "default-style"),
                Type.REFRESH => SetAttribute("http-equiv", "refresh"),
                _ => false
            };
        }

        public void SetContent(string content)
        {
            if (m_Type == Type.CHARSET)
                SetAttribute("charset", content);
            else
                SetAttribute("content", content);
        }
    }
}
