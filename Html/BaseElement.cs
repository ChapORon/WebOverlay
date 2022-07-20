using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebOverlay.Html
{
    public class BaseElement : IAppendable
    {
        private class AppendableString : IAppendable
        {
            private readonly string m_Str;

            public AppendableString(string str)
            {
                m_Str = str;
            }

            public void Append(ref StringBuilder builder)
            {
                builder.Append(m_Str);
            }
        }

        private readonly string m_Name;
        private static readonly List<string> ms_DirAllowed = new() { "ltr", "rtl", "auto" };
        private readonly Dictionary<string, AttributeValidationRule> m_AttributesStrictValidationRules = new()
        {
            {"accesskey", new IsTypeRule<char>()}, //Specifies a shortcut key to activate/focus an element
            {"class", new IsMatchingRegexRule("^[a-zA-Z]([a-zA-Z-_])*")}, //Specifies one or more classnames for an element(refers to a class in a style sheet)
            {"contenteditable", new IsTypeRule<bool>()}, //Specifies whether the content of an element is editable or not
            {"dir", new IsInListRule<string>(ms_DirAllowed)}, //Specifies the text direction for the content in an element
            {"draggable", new IsTypeRule<bool>()}, //Specifies whether an element is draggable or not
            {"hidden", new IsTypeRule<bool>()}, //Specifies that an element is not yet, or is no longer, relevant
            {"id", new(value => value is string str && !string.IsNullOrWhiteSpace(str) && !str.Contains(' '))}, //Specifies a unique id for an element
            {"lang", new IsLangRule()}, //Specifies the language of the element's content
            {"spellcheck", new IsTypeRule<bool>()}, //Specifies whether the element is to have its spelling and grammar checked or not
            {"style", new IsNonEmptyStringRule()}, //Specifies an inline CSS style for an element
            {"tabindex", new IsTypeRule<int>()}, //Specifies a shortcut key to activate/focus an element
            {"title", new IsTypeRule<string>()}, //Specifies a shortcut key to activate/focus an element
            {"translate", new IsTypeRule<bool>()} //Specifies whether the content of an element should be translated or not
        };
        private readonly Regex m_GlobalAttributeDataRegex = new("data-.*"); //Used to store custom data private to the page or application
        private readonly List<Tuple<Regex, AttributeValidationRule>> m_AttributeRegexValidationRules = new();
        private readonly bool m_AllowEventsAttributes = false;
        private readonly bool m_AllowUnknownAttributes = false;
        private readonly Dictionary<string, object> m_Attributes = new();
        private readonly List<IAppendable> m_Contents = new();

        protected BaseElement(string name, bool allowEventsAttributes = false, bool allowUnknownAttributes = false)
        {
            m_Name = name;
            m_AllowEventsAttributes = allowEventsAttributes;
            m_AllowUnknownAttributes = allowUnknownAttributes;
        }

        private bool CanEditAttributeRule(string attributeName)
        {
            if (m_AttributesStrictValidationRules.TryGetValue(attributeName, out var validation))
                return validation.IsEditable();
            else if (m_GlobalAttributeDataRegex.IsMatch(attributeName))
                return false;
            foreach (var regexValidationRule in m_AttributeRegexValidationRules)
            {
                if (regexValidationRule.Item1.IsMatch(attributeName))
                    return regexValidationRule.Item2.IsEditable();
            }
            return true;
        }

        protected void AddStrictValidationRule(Dictionary<string, AttributeValidationRule> rules)
        {
            foreach (var rule in rules)
            {
                if (!CanEditAttributeRule(rule.Key))
                    return;
                m_AttributesStrictValidationRules.Add(rule.Key, rule.Value);
            }
        }

        protected void AddRegexValidationRule(List<Tuple<string, AttributeValidationRule>> rules)
        {
            foreach (var rule in rules)
            {
                if (!CanEditAttributeRule(rule.Item1))
                    return;
                m_AttributeRegexValidationRules.Add(new(new(rule.Item1), rule.Item2));
            }
        }

        protected void AddStrictValidationRule(string attributeName, AttributeValidationRule rule)
        {
            if (!CanEditAttributeRule(attributeName))
                return;
            m_AttributesStrictValidationRules.Add(attributeName, rule);
        }

        protected void AddRegexValidationRule(string attributeRegex, AttributeValidationRule rule)
        {
            if (!CanEditAttributeRule(attributeRegex))
                return;
            m_AttributeRegexValidationRules.Add(new(new(attributeRegex), rule));
        }

        private bool IsAttributeValid(string name, object value)
        {
            if (m_GlobalAttributeDataRegex.IsMatch(name))
                return true;
            else if (m_AttributesStrictValidationRules.TryGetValue(name, out var validation))
                return validation.IsValid(value);
            foreach (var regexValidationRule in m_AttributeRegexValidationRules)
            {
                if (regexValidationRule.Item1.IsMatch(name))
                    return regexValidationRule.Item2.IsValid(value);
            }
            return m_AllowUnknownAttributes;
        }

        protected bool SetAttribute(string name, object value)
        {
            if (IsAttributeValid(name, value))
            {
                m_Attributes.Add(name, value);
                return true;
            }
            return false;
        }

        //
        //Global Attributes
        //
        public void SetAccessKey(char value) { SetAttribute("accesskey", value); }
        public void SetClass(string value) { SetAttribute("class", value); }
        public void SetContentEditable(bool value) { SetAttribute("contenteditable", value); }
        public void SetData(string name, object value) { SetAttribute(string.Format("data-{0}", name), value); }
        public void SetDir(string value) { SetAttribute("dir", value); }
        public void SetDraggable(bool value) { SetAttribute("draggable", value); }
        public void SetHidden(bool value) { SetAttribute("hidden", value); }
        public void SetID(string value) { SetAttribute("id", value); }
        public void SetLang(string value) { SetAttribute("lang", value); }
        public void SetSpellcheck(bool value) { SetAttribute("spellcheck", value); }
        public void SetStyle(string value) { SetAttribute("style", value); }
        public void SetTabIndex(int value) { SetAttribute("tabindex", value); }
        public void SetTitle(string value) { SetAttribute("title", value); }
        public void SetTranslate(bool value) { SetAttribute("translate", value); }

        protected void AddElement(BaseElement element)
        {
            m_Contents.Add(element);
        }

        protected void AddElement(string element)
        {
            m_Contents.Add(new AppendableString(element));
        }

        protected virtual bool IsEmpty()
        {
            return m_Attributes.Count == 0 && m_Contents.Count == 0;
        }

        protected void AppendHeadTag(ref StringBuilder builder)
        {
            builder.Append('<');
            builder.Append(m_Name);
            foreach (KeyValuePair<string, object> attribute in m_Attributes)
            {
                if (attribute.Value is bool value)
                {
                    if (value)
                    {
                        builder.Append(' ');
                        builder.Append(attribute.Key);
                    }
                }
                else
                {
                    builder.Append(' ');
                    builder.Append(attribute.Key);
                    builder.Append("=\"");
                    builder.Append(attribute.Value);
                    builder.Append('"');
                }
            }
            builder.Append('>');
        }

        protected void AppendTailTag(ref StringBuilder builder)
        {
            builder.Append("</");
            builder.Append(m_Name);
            builder.Append('>');
        }

        public override string ToString()
        {
            if (IsEmpty())
                return string.Format("<{0}/>", m_Name);
            StringBuilder builder = new();
            Append(ref builder);
            return builder.ToString();
        }

        public virtual void Append(ref StringBuilder builder)
        {
            AppendHeadTag(ref builder);
            foreach (IAppendable element in m_Contents)
                element.Append(ref builder);
            AppendTailTag(ref builder);
        }
    }
}
