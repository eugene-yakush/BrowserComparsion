using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;

namespace BrowserComparsion
{
    public class CefSharpController : BaseWebBrowserController
    {
        private readonly ChromiumWebBrowser _browserView;

        public CefSharpController(ChromiumWebBrowser browserView)
        {
            _browserView = browserView;
            _browserView.TitleChanged += OnActionsUpdated;
            _browserView.FrameLoadEnd += OnPageLoaded;
            _browserView.FrameLoadStart += OnActionsUpdated;
            _browserView.LoadingStateChanged += OnActionsUpdated;
            _browserView.StatusMessage += OnActionsUpdated;
        }

        private void OnPageLoaded(object sender, FrameLoadEndEventArgs e)
        {
            Navigated();
            OnActionsUpdated();
        }

        private void OnActionsUpdated(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnActionsUpdated();
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        public override string CurrentUrl => _browserView.Address;

        public override void Navigate(string url)
        {
            base.Navigate(url);
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    _browserView.Load(url);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            ));
        }

        public override bool CanGoBack => _browserView.CanGoBack;
        public override void Back()
        {
            _browserView.Back();
        }

        public override bool CanGoForward => _browserView.CanGoForward;
        public override void Forward()
        {
            _browserView.Forward();
        }

        public override void Refresh()
        {
            _browserView.Reload();
        }

        public override List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public override void ExecuteJavascript(string script)
        {
            _browserView.ExecuteScriptAsync(script);
        }

        public override string ExecuteJavascriptWithResult(string script)
        {
            throw new NotSupportedException();
        }

        public override string GetHtml()
        {
            return _browserView.GetSourceAsync().Result;
        }

        public override void SetHtml(string html)
        {
            _browserView.LoadHtml(html, "html");
        }

        public override List<string> GetDomById(string id)
        {
            throw new NotSupportedException();
        }

        public override List<string> GetDomByTag(string id)
        {
            throw new NotSupportedException();
        }

        public override List<string> GetDomByClass(string id)
        {
            throw new NotSupportedException();
        }

        public override void SetDomById(string id, string html)
        {
            throw new NotSupportedException();
        }
    }   
}