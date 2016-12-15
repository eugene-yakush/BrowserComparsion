using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
}
