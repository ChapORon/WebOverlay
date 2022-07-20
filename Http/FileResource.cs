namespace WebOverlay.Http
{
    public class FileResource: Resource
    {
        private string m_Path = "";

        public FileResource() {}

        public FileResource(string path)
        {
            m_Path = path;
        }

        protected override void OnInit(params dynamic[] args)
        {
            if (args.Length >= 1)
                m_Path = args[0];
        }

        protected override void Get(int clientID, Request request)
        {
            if (!File.Exists(m_Path))
            {
                SendError(clientID, Defines.NewResponse(request.Version, 404));
                return;
            }
            SendResponse(clientID, Defines.NewResponse(request.Version, 200, File.ReadAllText(m_Path)));
        }
    }
}
