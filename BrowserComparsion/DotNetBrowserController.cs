using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DotNetBrowser;
using DotNetBrowser.DOM;
using DotNetBrowser.Events;
using DotNetBrowser.WPF;

namespace BrowserComparsion
{
    public class DotNetBrowserController : BaseWebBrowserController
    {
        private readonly WPFBrowserView _browserView;

        public DotNetBrowserController(WPFBrowserView browserView)
        {
            _browserView = browserView;
            _browserView.DocumentLoadedInMainFrameEvent += OnPageLoaded;
            _browserView.StatusChangedEvent += OnActionsUpdated;
            _browserView.TitleChangedEvent += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        private void OnPageLoaded(object sender, EventArgs e)
        {
            Navigated();
            OnActionsUpdated();
        }

        public override string CurrentUrl => _browserView.URL;

        public override void Navigate(string url)
        {
            base.Navigate(url);
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    _browserView.Browser.LoadURL(url);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            ));
        }

        public override bool CanGoBack => _browserView.Browser.CanGoBack();

        public override void Back()
        {
            _browserView.Browser.GoBack();
        }

        public override bool CanGoForward => _browserView.Browser.CanGoForward();
        public override void Forward()
        {
            _browserView.Browser.GoForward();
        }

        public override void Refresh()
        {
            _browserView.Browser.Reload();
        }

        public override List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.JsExecuteWithResult,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml,
            BrowserFeature.GetDom,
            BrowserFeature.SetDom
        };

        public override void ExecuteJavascript(string script)
        {
            _browserView.Browser.ExecuteJavaScript(script);
        }

        public override string ExecuteJavascriptWithResult(string script)
        {
            return _browserView.Browser.ExecuteJavaScriptAndReturnValue(script).ToString();
        }

        public override string GetHtml()
        {
            return _browserView.Browser.GetHTML();
        }

        public override void SetHtml(string html)
        {
            _browserView.Browser.LoadHTML(html);
        }

        public override List<string> GetDomById(string id)
        {
            List<string> res = new List<string>();
            DOMElement element = _browserView.Browser.GetDocument().GetElementById(id);
            if(element != null)
                res.Add(element.InnerHTML);
            return res;
        }

        public override List<string> GetDomByTag(string id)
        {
            return _browserView.Browser.GetDocument().GetElementsByTagName(id).Select(e => (e as DOMElement)?.InnerHTML?.Trim()).ToList();
        }

        public override List<string> GetDomByClass(string id)
        {
            return _browserView.Browser.GetDocument().GetElementsByClassName(id).Select(e => (e as DOMElement)?.InnerHTML?.Trim()).ToList();
        }

        public override void SetDomById(string id, string html)
        {
            _browserView.Browser.GetDocument().GetElementById(id).SetInnerHTML(html);
        }
    }

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

    public class PerformanceCounter
    {
        private List<IBrowserController> controllers;
        private bool abort = false;

        public bool Running { get; private set; }
        public event Action DataUpdated;        

        public PerformanceCounter(List<IBrowserController> controllers)
        {
            this.controllers = controllers;
        }

        public void Measure(List<PerformanceEntry> entries)
        {
            Task.Run(() =>
            {
                abort = false;
                Running = true;
                ManualResetEvent controllerComplete = new ManualResetEvent(false);


                foreach (PerformanceEntry performanceEntry in entries)
                {
                    foreach (IBrowserController browserController in controllers)
                    {
                        Action<int> resultWriteAction = null;
                        if (browserController is DotNetBrowserController) resultWriteAction = (result) => performanceEntry.DotNetBrowserPeriod = result;
                        if (browserController is WebBrowserController) resultWriteAction = (result) => performanceEntry.WebBrowserPeriod = result;
                        if (browserController is AwesomiumController) resultWriteAction = (result) => performanceEntry.AwesomiumPeriod = result;
                        if (browserController is CefSharpController) resultWriteAction = (result) => performanceEntry.CefSharpPeriod = result;

                        controllerComplete.Reset();
                        browserController.Navigate(performanceEntry.Url, (period) => { resultWriteAction(period); controllerComplete.Set(); });
                        bool normal = controllerComplete.WaitOne(30000);
                        if (!normal)
                        {
                            resultWriteAction(30000);
                            controllerComplete.Set();
                        }
                        OnDataUpdated();

                        if (abort) break;
                    }
                    if (abort) break;
                }

                Running = false;
                OnDataUpdated();
            });
        }

        protected virtual void OnDataUpdated()
        {
            DataUpdated?.Invoke();
        }

        public void Abort()
        {
            abort = true;
        }
    }
}
