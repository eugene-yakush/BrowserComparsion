using System;
using System.ComponentModel;

namespace BrowserComparsion
{
    [Serializable]
    public class PerformanceEntry
    {
        public string Url { get; set; }

        [DisplayName("DotNetBrowser")]
        public int DotNetBrowserPeriod { get; set; }

        [DisplayName("WebBrowser")]
        public int WebBrowserPeriod { get; set; }

        [DisplayName("Awesomium")]
        public int AwesomiumPeriod { get; set; }

        [DisplayName("CefSharp")]
        public int CefSharpPeriod { get; set; }        
    }
}