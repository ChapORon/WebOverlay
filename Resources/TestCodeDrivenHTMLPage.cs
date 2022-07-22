using WebOverlay.Html;

namespace WebOverlay.Resources
{
    public class TestCodeDrivenHTMLPage : Http.CodeDrivenHtmlResource
    {
        protected override void Generate()
        {
            Head head = m_Page.GetHead();
            head.SetCharset("UTF-8");
            head.AddMeta(Meta.Type.VIEWPORT, "width=1920px,height=1080px");

            Script framework = new();
            framework.SetSrc("/js/woframework.js");
            head.AddScript(framework);

            Script script1 = new();
            script1.SetSrc("/js/test.js");
            head.AddScript(script1);

            Body body = m_Page.GetBody();
            body.AddAttribute("onload", "wof.main();onBlinkLoad()");
            Div blinkDiv = new();
            blinkDiv.SetID("blink");
            blinkDiv.AddContent("Blink");
            body.AddChild(blinkDiv);
            body.AddLineBreak();
            Tag gifDiv = new("img");
            gifDiv.AddAttribute("src", "http://i.stack.imgur.com/SBv4T.gif");
            gifDiv.AddAttribute("alt", "this slowpoke moves");
            gifDiv.AddAttribute("width", 250);
            body.AddChild(gifDiv);
            body.AddLineBreak();
            body.AddContent("This is a ");
            Tag link = new("a");
            link.AddAttribute("href", "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            link.AddContent("link");
            body.AddChild(link);
            body.AddContent(" without line break");
            body.AddContent("\nThis is line 1\nThis is line 2\n");
            body.AddContent("This is another line");
        }

        protected override void WebsocketMessage(int clientID, string message)
        {
            MessageBox.Show(message);
        }
    }
}
