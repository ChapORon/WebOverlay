namespace WebOverlay.Html
{
    public class Title : BaseElement
    {
        public Title(string title) : base("title")
        {
            AddContent(title);
        }
    }
}
