namespace WebOverlay.Html
{
    public class Base : BaseElement
    {
        public enum Type
        {
            BLANK,
            PARENT,
            SELF,
            TOP,
            NONE
        }

        public Base() : base("base", isEmptyTag: true)
        {
            AddStrictValidationRule(new()
            {
                {"href", new IsNonEmptyStringRule()}, //Specifies the base URL for all relative URLs in the page
                {"target", new IsInListRule<string>(new(){"_blank", "_parent", "_self", "_top"})} //Specifies the default target for all hyperlinks and forms in the page
            });
        }

        public void SetBase(string href, Type target)
        {
            if (string.IsNullOrWhiteSpace(href))
                SetAttribute("href", href);
            else
                RemoveAttribute("href");
            _ = target switch
            {
                Type.BLANK => SetAttribute("target", "_blank"),
                Type.PARENT => SetAttribute("target", "_parent"),
                Type.SELF => SetAttribute("target", "_self"),
                Type.TOP => SetAttribute("target", "_top"),
                _ => RemoveAttribute("target")
            };
        }
    }
}
