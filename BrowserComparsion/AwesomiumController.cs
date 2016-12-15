using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesomium.Core;
using Awesomium.Windows.Controls;

namespace BrowserComparsion
{
    public class AwesomiumController : IBrowserController
    {
        private readonly WebControl _browserView;

        public event Action ActionsUpdated;

        public AwesomiumController(WebControl browserView)
        {
            _browserView = browserView;
            _browserView.AddressChanged += OnActionsUpdated;
            _browserView.TargetURLChanged += OnActionsUpdated;
            _browserView.TitleChanged += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        protected virtual void OnActionsUpdated()
        {
            ActionsUpdated?.Invoke();
        }

        public string CurrentUrl => _browserView.Source.ToString();

        public void Navigate(string url)
        {
            _browserView.Source = new Uri(url);
        }

        public bool CanGoBack => _browserView.CanGoBack();
        public void Back()
        {
            _browserView.GoBack();
        }

        public bool CanGoForward => _browserView.CanGoForward();
        public void Forward()
        {
            _browserView.GoForward();
        }

        public void Refresh()
        {
            _browserView.Reload(false);
        }

        public List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.JsExecuteWithResult,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public void ExecuteJavascript(string script)
        {
            _browserView.ExecuteJavascript(script);
        }

        public string ExecuteJavascriptWithResult(string script)
        {
            return _browserView.ExecuteJavascriptWithResult(script).ToString();
        }

        public string GetHtml()
        {
            return _browserView.HTML;
        }

        public void SetHtml(string html)
        {
            _browserView.LoadHTML(html);
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