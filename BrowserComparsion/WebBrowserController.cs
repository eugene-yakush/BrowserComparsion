using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;

namespace BrowserComparsion
{
    public class WebBrowserController : IBrowserController
    {
        private readonly WebBrowser _browserView;

        public event Action ActionsUpdated;

        public WebBrowserController(WebBrowser browserView)
        {
            _browserView = browserView;
            _browserView.LoadCompleted += OnActionsUpdated;
            _browserView.Navigated += OnActionsUpdated;
            _browserView.Navigating += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        protected virtual void OnActionsUpdated()
        {
            ActionsUpdated?.Invoke();
        }

        public string CurrentUrl => _browserView?.Source?.ToString();

        public void Navigate(string url)
        {
            _browserView.Navigate(new Uri(url));
            OnActionsUpdated();
        }

        public bool CanGoBack => _browserView.CanGoBack;
        public void Back()
        {
            _browserView.GoBack();
            OnActionsUpdated();
        }

        public bool CanGoForward => _browserView.CanGoForward;
        public void Forward()
        {
            _browserView.GoForward();
            OnActionsUpdated();
        }

        public void Refresh()
        {
            _browserView.Refresh();
            OnActionsUpdated();
        }

        public List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public void ExecuteJavascript(string script)
        {
            throw new NotSupportedException();
        }

        public string ExecuteJavascriptWithResult(string script)
        {
            throw new NotSupportedException();
        }

        public string GetHtml()
        {
            return (_browserView.Document as IHTMLDocument2)?.body?.outerHTML;
        }

        public void SetHtml(string html)
        {
            _browserView.NavigateToString(html);
        }

        public List<string> GetDomById(string id)
        {
            throw new NotSupportedException();
        }

        public List<string> GetDomByTag(string id)
        {
            throw new NotSupportedException();
        }

        public List<string> GetDomByClass(string id)
        {
            throw new NotSupportedException();
        }

        public void SetDomById(string id, string html)
        {
            throw new NotSupportedException();
        }
    }
}