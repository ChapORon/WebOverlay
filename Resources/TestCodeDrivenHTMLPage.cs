using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOverlay.Resources
{
    public class TestCodeDrivenHTMLPage : Http.CodeDrivenHtmlResource
    {
        protected override void Generate()
        {
            Html.Head head = m_Page.GetHead();
            head.SetCharset("UTF-8");

            Html.Script framework = new();
            framework.SetSrc("/js/woframework.js");
            head.AddScript(framework);

            Html.Script script1 = new();
            script1.SetSrc("/js/test.js");
            head.AddScript(script1);

            Html.Element body = m_Page.GetBody();
            body.AddAttribute("onload", "wof.main()");
            body.AddContent("<div onload=\"onBlinkLoad()\" id=\"blink\">Blink</div>");
            body.AddContent("<br/>");
            body.AddContent("<img src=\"http://i.stack.imgur.com/SBv4T.gif\" alt=\"this slowpoke moves\" width=\"250\"/>");
        }

        protected override void WebsocketMessage(int clientID, string message)
        {
            MessageBox.Show(message);
        }
    }
}
