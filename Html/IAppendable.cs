using System.Text;

namespace WebOverlay.Html
{
    internal interface IAppendable
    {
        void Append(ref StringBuilder builder);
    }
}
