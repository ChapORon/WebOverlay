namespace WebOverlay.Html
{
    public class Element : BaseElement
    {
        public Element(string name): base(name, allowEventsAttributes: true, allowUnknownAttributes: true) {}

        public bool AddAttribute(string name, object value)
        {
            return SetAttribute(name, value);
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
