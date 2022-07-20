using System.Text.RegularExpressions;

namespace WebOverlay.Html
{
    public class Link: BaseElement
    {
        public enum RelationType
        {
            ALTERNATE,
            AUTHOR,
            DNS_PREFETCH,
            HELP,
            ICON,
            LICENSE,
            NEXT,
            PINGBACK,
            PRECONNECT,
            PREFETCH,
            PRELOAD,
            PRERENDER,
            PREV,
            SEARCH,
            STYLESHEET
        }

        private static readonly List<string> ms_CrossOriginAllowed = new() { "anonymous", "use-credentials" };
        private static readonly List<string> ms_ReferrerpolicyAllowed = new() { "no-referrer", "no-referrer-when-downgrade", "origin", "origin-when-cross-origin", "unsafe-url" };
        private static readonly List<string> ms_RelAllowed = new() { "alternate", "author", "dns-prefetch", "help", "icon", "license", "next", "pingback", "preconnect", "prefetch", "preload", "prerender", "prev", "search", "stylesheet" };

        private static string GetRelName(RelationType relationType)
        {
            switch (relationType)
            {
                case RelationType.ALTERNATE: return "alternate";
                case RelationType.AUTHOR: return "author";
                case RelationType.DNS_PREFETCH: return "dns-prefetch";
                case RelationType.HELP: return "help";
                case RelationType.ICON: return "icon";
                case RelationType.LICENSE: return "license";
                case RelationType.NEXT: return "next";
                case RelationType.PINGBACK: return "pingback";
                case RelationType.PRECONNECT: return "preconnect";
                case RelationType.PREFETCH: return "prefetch";
                case RelationType.PRELOAD: return "preload";
                case RelationType.PRERENDER: return "prerender";
                case RelationType.PREV: return "prev";
                case RelationType.SEARCH: return "search";
                case RelationType.STYLESHEET: return "stylesheet";
            }
            return "";
        }

        private static readonly Regex ms_SizesRegex = new("[0-9]+x[0-9]+|any");

        public Link(RelationType relationType): base("link")
        {
            AddStrictValidationRule(new()
            {
                {"crossorigin", new IsInListRule<string>(ms_CrossOriginAllowed)}, //Specifies how the element handles cross-origin requests
                {"href", new IsNonEmptyStringRule()}, //Specifies the location of the linked document
                {"hreflang", new IsLangRule()}, //Specifies the language of the text in the linked document
                {"media", new IsNonEmptyStringRule()}, //Specifies on what device the linked document will be displayed
                {"referrerpolicy", new IsInListRule<string>(ms_ReferrerpolicyAllowed)}, //Specifies which referrer to use when fetching the resource
                {"rel", new IsInListRule<string>(ms_RelAllowed)}, //Required. Specifies the relationship between the current document and the linked document
                {"sizes", new IsMatchingRegexRule("^[0-9]+x[0-9]+|any")}, //Specifies the size of the linked resource. Only for rel="icon"
                {"type", new IsNonEmptyStringRule()}, //Specifies the media type of the linked document
            });
            SetAttribute("rel", GetRelName(relationType));
        }

        public void SetCrossOrigin(string value) { SetAttribute("crossorigin", value); }
        public void SetHref(string value) { SetAttribute("href", value); }
        public void SetHrefLang(string value) { SetAttribute("hreflang", value); }
        public void SetMedia(string value) { SetAttribute("media", value); }
        public void SetReferrerPolicy(string value) { SetAttribute("referrerpolicy", value); }
        public void SetRel(RelationType value) { SetAttribute("rel", GetRelName(value)); }
        public void SetSizes(int width, int height) { SetAttribute("sizes", string.Format("{0}x{1}", width, height)); }
        public void SetAnySizes() { SetAttribute("sizes", "any"); }
        public void SetType(string value) { SetAttribute("type", value); }
    }
}
