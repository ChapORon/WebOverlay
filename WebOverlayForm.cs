using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace WebOverlay
{
    public partial class WebOverlayForm : Form
    {
        private readonly Stopwatch m_Watch = new();
        private readonly Http.Server m_WebServer = new();

        public WebOverlayForm()
        {
            InitializeComponent();
            m_WebServer.AddResource<Resources.Index>("/");
            m_WebServer.AddResource<Resources.TestCodeDrivenHTMLPage>("/test", "Document");
            m_WebServer.AddResource<Http.Resource>("/ramo");
            m_WebServer.AddResource("/js", "D:\\Programmation\\WebOverlay\\Js");
            m_WebServer.AddResource("/", "D:\\Programmation\\WebOverlay\\favicon.ico", true);
            m_WebServer.StartListening();

            Timer timer = new()
            {
                Interval = 50 //milliseconds
            };
            timer.Tick += WebOverlayForm_Tick;
            m_Watch.Start();
            timer.Start();
        }

        private void WebOverlayForm_Tick(object? sender, EventArgs e)
        {
            m_Watch.Stop();
            m_WebServer.Update(m_Watch.ElapsedMilliseconds);
            m_Watch.Reset();
            m_Watch.Start();
        }
    }
}