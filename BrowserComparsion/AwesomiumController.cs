using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Awesomium.Core;
using Awesomium.Windows.Controls;

namespace BrowserComparsion
{
    public class AwesomiumController : BaseWebBrowserController
    {
        private readonly WebControl _browserView;

        public AwesomiumController(WebControl browserView)
        {
            _browserView = browserView;
            _browserView.LoadingFrameComplete += OnPageLoaded;
            _browserView.AddressChanged += OnActionsUpdated;
            _browserView.TargetURLChanged += OnActionsUpdated;
            _browserView.TitleChanged += OnActionsUpdated;
        }

        private void OnActionsUpdated(object sender, EventArgs e)
        {
            OnActionsUpdated();
        }

        private void OnPageLoaded(object sender, FrameEventArgs e)
        {
            Navigated();
            OnActionsUpdated();
        }

        public override string CurrentUrl => _browserView.Source.ToString();

        public override void Navigate(string url)
        {
            base.Navigate(url);
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    _browserView.Source = new Uri(url);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            ));
        }

        public override bool CanGoBack => _browserView.CanGoBack();
        public override void Back()
        {
            _browserView.GoBack();
        }

        public override bool CanGoForward => _browserView.CanGoForward();
        public override void Forward()
        {
            _browserView.GoForward();
        }

        public override void Refresh()
        {
            _browserView.Reload(false);
        }

        public override List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.JsExecute,
            BrowserFeature.JsExecuteWithResult,
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public override void ExecuteJavascript(string script)
        {
            _browserView.ExecuteJavascript(script);
        }

        public override string ExecuteJavascriptWithResult(string script)
        {
            return _browserView.ExecuteJavascriptWithResult(script).ToString();
        }

        public override string GetHtml()
        {
            return _browserView.HTML;
        }

        public override void SetHtml(string html)
        {
            _browserView.LoadHTML(html);
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