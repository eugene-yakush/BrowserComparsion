using System;
using System.Collections.Generic;
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
    public class DotNetBrowserController : IBrowserController
    {
        private readonly WPFBrowserView _browserView;
        
        public event Action ActionsUpdated;

        public DotNetBrowserController(WPFBrowserView browserView)
        {
            _browserView = browserView;
            _browserView.DocumentLoadedInMainFrameEvent += OnActionsUpdated;
            _browserView.StatusChangedEvent += OnActionsUpdated;
            _browserView.TitleChangedEvent += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        protected virtual void OnActionsUpdated()
        {
            ActionsUpdated?.Invoke();
        }

        public string CurrentUrl => _browserView.URL;

        public void Navigate(string url)
        {
            _browserView.Browser.LoadURL(url);
        }

        public bool CanGoBack => _browserView.Browser.CanGoBack();
        public void Back()
        {
            _browserView.Browser.GoBack();
        }

        public bool CanGoForward => _browserView.Browser.CanGoForward();
        public void Forward()
        {
            _browserView.Browser.GoForward();
        }

        public void Refresh()
        {
            _browserView.Browser.Reload();
        }

        public List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.JsExecuteWithResult,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml,
            BrowserFeature.GetDom,
            BrowserFeature.SetDom
        };

        public void ExecuteJavascript(string script)
        {
            _browserView.Browser.ExecuteJavaScript(script);
        }

        public string ExecuteJavascriptWithResult(string script)
        {
            return _browserView.Browser.ExecuteJavaScriptAndReturnValue(script).ToString();
        }

        public string GetHtml()
        {
            return _browserView.Browser.GetHTML();
        }

        public void SetHtml(string html)
        {
            _browserView.Browser.LoadHTML(html);
        }

        public List<string> GetDomById(string id)
        {
            List<string> res = new List<string>();
            DOMElement element = _browserView.Browser.GetDocument().GetElementById(id);
            if(element != null)
                res.Add(element.InnerHTML);
            return res;
        }

        public List<string> GetDomByTag(string id)
        {
            return _browserView.Browser.GetDocument().GetElementsByTagName(id).Select(e => (e as DOMElement)?.InnerHTML?.Trim()).ToList();
        }

        public List<string> GetDomByClass(string id)
        {
            return _browserView.Browser.GetDocument().GetElementsByClassName(id).Select(e => (e as DOMElement)?.InnerHTML?.Trim()).ToList();
        }

        public void SetDomById(string id, string html)
        {
            _browserView.Browser.GetDocument().GetElementById(id).SetInnerHTML(html);
        }
    }
}
