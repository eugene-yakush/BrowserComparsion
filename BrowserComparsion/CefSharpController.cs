using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;

namespace BrowserComparsion
{
    public class CefSharpController : IBrowserController
    {
        private readonly ChromiumWebBrowser _browserView;

        public event Action ActionsUpdated;

        public CefSharpController(ChromiumWebBrowser browserView)
        {
            _browserView = browserView;
            _browserView.TitleChanged += OnActionsUpdated;
            _browserView.FrameLoadEnd += OnActionsUpdated;
            _browserView.FrameLoadStart += OnActionsUpdated;
            _browserView.LoadingStateChanged += OnActionsUpdated;
            _browserView.StatusMessage += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnActionsUpdated();
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        protected virtual void OnActionsUpdated()
        {
            ActionsUpdated?.Invoke();
        }

        public string CurrentUrl => _browserView.Address;

        public void Navigate(string url)
        {
            _browserView.Load(url);
        }

        public bool CanGoBack => _browserView.CanGoBack;
        public void Back()
        {
            _browserView.Back();
        }

        public bool CanGoForward => _browserView.CanGoForward;
        public void Forward()
        {
            _browserView.Forward();
        }

        public void Refresh()
        {
            _browserView.Reload();
        }

        public List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public void ExecuteJavascript(string script)
        {
            _browserView.ExecuteScriptAsync(script);
        }

        public string ExecuteJavascriptWithResult(string script)
        {
            throw new NotSupportedException();
        }

        public string GetHtml()
        {
            return _browserView.GetSourceAsync().Result;
        }

        public void SetHtml(string html)
        {
            _browserView.LoadHtml(html, "html");
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