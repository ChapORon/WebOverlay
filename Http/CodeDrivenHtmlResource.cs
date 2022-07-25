using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOverlay.Http
{
    public abstract class CodeDrivenHtmlResource : Resource
    {
        protected Html.Document m_Page = new("");

        public CodeDrivenHtmlResource(): base() { }
        protected CodeDrivenHtmlResource(bool needUpdate) : base(needUpdate) { }

        protected override void OnInit(params dynamic[] args)
        {
            if (args.Length >= 1)
            {
                m_Page = new(args[0]);
                Generate();
            }
        }

        protected override void Get(int clientID, Request request)
        {
            if (m_Page != null)
                SendResponse(clientID, Defines.NewResponse(request.Version, 200, m_Page.ToString()));
        }

        protected abstract void Generate();
    }
}
