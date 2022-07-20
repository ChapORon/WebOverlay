namespace WebOverlay
{
    public partial class WebOverlayForm : Form
    {
        private readonly Http.Server m_WebServer = new(4242);

        public WebOverlayForm()
        {
            InitializeComponent();
            m_WebServer.AddResource<Resources.Index>("/");
            m_WebServer.AddResource<Resources.TestCodeDrivenHTMLPage>("/test", "Document");
            m_WebServer.AddResource<Http.Resource>("/ramo");
            m_WebServer.AddResource<Http.FileResource>("/js/woframework.js", "D:\\Programmation\\WebOverlay\\Js\\js\\woframework.js");
            m_WebServer.AddResource<Http.FileResource>("/js/test.js", "D:\\Programmation\\WebOverlay\\Js\\js\\test.js");
            m_WebServer.AddResource<Http.FileResource>("/favicon.ico", "D:\\Programmation\\WebOverlay\\favicon.ico");
            m_WebServer.StartListening();
        }
    }
}