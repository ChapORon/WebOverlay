using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebOverlay.Html
{
    public class BaseElement
    {
        private class Placeholder : BaseElement
        {
            public readonly int ContentPos;
            public Placeholder(int pos): base("__placeholder__") { ContentPos = pos; }
        }

        //                                [event,  tags,         isAllowList]
        private static readonly List<Tuple<string, List<string>, bool>> ms_EventsRules = new()
        {
            //Window events
            {new("onafterprint", new(){"body"}, true)},
            {new("onbeforeprint", new(){"body"}, true)},
            {new("onbeforeunload", new(){"body"}, true)},
            {new("onhashchange", new(){"body"}, true)},
            {new("onload", new(){"body", "frame", "frameset", "iframe", "img", "input", "link", "script", "style"}, true)},
            {new("onmessage", new(){"body"}, true)},
            {new("onoffline", new(){"body"}, true)},
            {new("ononline", new(){"body"}, true)},
            {new("onpagehide", new(){"body"}, true)},
            {new("onpageshow", new(){"body"}, true)},
            {new("onpopstate", new(){"body"}, true)},
            {new("onresize", new(){"body"}, true)},
            {new("onstorage", new(){"body"}, true)},
            {new("onunload", new(){"body"}, true)},

            //Form events
            {new("onblur", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onchange", new(){"input", "select", "textarea"}, true)},
            {new("oncontextmenu", new(), false)},
            {new("onfocus", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onimput", new(){"input", "textarea"}, true)},
            {new("oninvalid", new(){"input"}, true)},
            {new("onreset", new(){"form"}, true)},
            {new("onsearch", new(){"input"}, true)},
            {new("onselect", new(){"input", "textarea"}, true)},
            {new("onsubmit", new(){"form"}, true)},

            //Keyboard events
            {new("onkeydown", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onkeypress", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onkeyup", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},

            //Mouse events
            {new("onclick", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("ondblclick", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onmousedown", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onmousemove", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onmouseout", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onmouseover", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onmouseup", new(){"base", "bdo", "br", "head", "html", "iframe", "meta", "param", "script", "style", "title"}, false)},
            {new("onwheel", new(), false)},

            //Drag events
            {new("ondrag", new(), false)},
            {new("ondragend", new(), false)},
            {new("ondragenter", new(), false)},
            {new("ondragleave", new(), false)},
            {new("ondragover", new(), false)},
            {new("ondragstart", new(), false)},
            {new("ondrop", new(), false)},
            {new("onscroll", new(){"address", "blockquote", "body", "caption", "center", "dd", "dir", "div", "dl", "dt", "fieldset", "form", "h1> to <h6", "html", "li", "menu", "object", "ol", "p", "pre", "select", "tbody", "textarea", "tfoot", "thead", "ul"}, true)},

            //Clipboard events
            {new("oncopy", new(), false)},
            {new("oncut", new(), false)},
            {new("onpaste", new(), false)},

            //Media events
            {new("onabort", new(), false)},
            {new("oncanplay", new(), false)},
            {new("oncanplaythrough", new(), false)},
            {new("oncuechange", new(), false)},
            {new("ondurationchange", new(), false)},
            {new("onemptied", new(), false)},
            {new("onended", new(), false)},
            {new("onerror", new(), false)},
            {new("onloadeddata", new(), false)},
            {new("onloadedmetadata", new(), false)},
            {new("onloadstart", new(), false)},
            {new("onpause", new(), false)},
            {new("onplay", new(), false)},
            {new("onplaying", new(), false)},
            {new("onprogress", new(), false)},
            {new("onratechange", new(), false)},
            {new("onseeked", new(), false)},
            {new("onseeking", new(), false)},
            {new("onstalled", new(), false)},
            {new("onsuspend", new(), false)},
            {new("ontimeupdate", new(), false)},
            {new("onvolumechange", new(), false)},
            {new("onwaiting", new(), false)},

            //Misc events
            {new("ontoggle", new(){"details"}, true)}
        };

        private readonly string m_Name;
        private readonly Dictionary<string, AttributeValidationRule> m_AttributesStrictValidationRules = new()
        {
            {"accesskey", new IsTypeRule<char>()}, //Specifies a shortcut key to activate/focus an element
            {"class", new IsMatchingRegexRule("^[a-zA-Z]([a-zA-Z-_])*")}, //Specifies one or more classnames for an element(refers to a class in a style sheet)
            {"contenteditable", new IsTypeRule<bool>()}, //Specifies whether the content of an element is editable or not
            {"dir", new IsInListRule<string>(new(){"ltr", "rtl", "auto"})}, //Specifies the text direction for the content in an element
            {"draggable", new IsTypeRule<bool>()}, //Specifies whether an element is draggable or not
            {"hidden", new IsTypeRule<bool>()}, //Specifies that an element is not yet, or is no longer, relevant
            {"id", new CallbackRule(value => value is string str && !string.IsNullOrWhiteSpace(str) && !str.Contains(' '))}, //Specifies a unique id for an element
            {"lang", new IsLangRule()}, //Specifies the language of the element's content
            {"spellcheck", new IsTypeRule<bool>()}, //Specifies whether the element is to have its spelling and grammar checked or not
            {"style", new IsNonEmptyStringRule()}, //Specifies an inline CSS style for an element
            {"tabindex", new IsTypeRule<int>()}, //Specifies a shortcut key to activate/focus an element
            {"title", new IsTypeRule<string>()}, //Specifies a shortcut key to activate/focus an element
            {"translate", new IsTypeRule<bool>()} //Specifies whether the content of an element should be translated or not
        };
        private readonly Regex m_GlobalAttributeDataRegex = new("data-.*"); //Used to store custom data private to the page or application
        private readonly List<Tuple<Regex, AttributeValidationRule>> m_AttributeRegexValidationRules = new();
        private readonly bool m_IsEmptyTag;
        private readonly bool m_AllowUnknownAttributes;
        private readonly Dictionary<string, object> m_Attributes = new();
        private readonly List<string> m_Contents = new();
        private readonly List<BaseElement> m_Childs = new();

        protected BaseElement(string name, bool isEmptyTag = false, bool allowUnknownAttributes = false)
        {
            m_Name = name;
            m_IsEmptyTag = isEmptyTag;
            m_AllowUnknownAttributes = allowUnknownAttributes;

            foreach (var eventRule in ms_EventsRules)
            {
                bool isAllowed = (eventRule.Item2.Contains(m_Name) == eventRule.Item3);
                m_AttributesStrictValidationRules.Add(eventRule.Item1, new EventAttributeRule(isAllowed));
                if (isAllowed && eventRule.Item1 == "oncontextmenu")
                    m_AttributesStrictValidationRules.Add("contextmenu", new IsNonEmptyStringRule());
            }
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

        protected bool RemoveAttribute(string name)
        {
            return m_Attributes.Remove(name);
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
            m_Childs.Add(element);
        }

        protected void AddElement(string element)
        {
            string[] lines = element.Split('\n');
            int lineIdx = 0;
            foreach (string line in lines)
            {
                if (lineIdx != 0)
                    m_Childs.Add(new LineBreak());
                string trimmedLine = line.Trim();
                if (trimmedLine.Length > 0)
                {
                    m_Childs.Add(new Placeholder(m_Contents.Count));
                    m_Contents.Add(trimmedLine);
                }
                ++lineIdx;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            Append(ref builder, 0);
            return builder.ToString();
        }

        protected void Append(ref StringBuilder builder, int tab)
        {
            builder.Append('<');
            builder.Append(m_Name);
            if (m_Attributes.Count == 0 && m_Childs.Count == 0)
            {
                builder.Append("/>");
                return;
            }
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
            if (!m_IsEmptyTag)
            {
                foreach (BaseElement child in m_Childs)
                {
                    if (child is Placeholder placeholder)
                    {
                        if (m_Contents.Count > 1)
                        {
                            builder.AppendLine();
                            builder.Append('\t', tab + 1);
                        }
                        builder.Append(m_Contents[placeholder.ContentPos]);
                    }
                    else
                    {
                        builder.AppendLine();
                        builder.Append('\t', tab + 1);
                        child.Append(ref builder, tab + 1);
                    }
                }
                if (m_Childs.Count != 0 && (m_Childs.Count != 1 || m_Contents.Count != 1))
                {
                    builder.AppendLine();
                    builder.Append('\t', tab);
                }
                builder.Append("</");
                builder.Append(m_Name);
                builder.Append('>');
            }
        }
    }
}
