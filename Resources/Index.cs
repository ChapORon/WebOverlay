namespace WebOverlay.Resources
{
    public class Index: Http.Resource
    {
        protected override void Get(int clientID, Http.Request request)
        {
            SendResponse(clientID, Http.Defines.NewResponse(request.Version, 200, "<p>Hello World !</p>"));
        }
    }
}
