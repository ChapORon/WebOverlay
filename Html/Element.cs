namespace WebOverlay.Html
{
    public class Element : BaseElement
    {
        protected Element(string name, bool allowUnknownAttributes) : base(name, allowUnknownAttributes: allowUnknownAttributes) { }

        public void AddAttribute(string name, object value)
        {
            SetAttribute(name, value);
        }

        public void AddLineBreak()
        {
            AddElement(new LineBreak());
        }

        public void AddChild(BaseElement element)
        {
            AddElement(element);
        }

        public void AddContent(string element)
        {
            AddElement(element);
        }
    }
}
