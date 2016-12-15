using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using mshtml;

namespace BrowserComparsion
{
    public class WebBrowserController : BaseWebBrowserController
    {
        private readonly WebBrowser _browserView;

        public WebBrowserController(WebBrowser browserView)
        {
            _browserView = browserView;
            HideScriptErrors(_browserView, true);
            _browserView.LoadCompleted += OnPageLoaded;
            _browserView.Navigated += OnActionsUpdated;
            _browserView.Navigating += OnActionsUpdated;
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
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

        public override string CurrentUrl => _browserView?.Source?.ToString();

        public override void Navigate(string url)
        {
            base.Navigate(url);
            App.Current.Dispatcher.Invoke((Action) (() =>
                {
                    try
                    {
                        _browserView.Navigate(new Uri(url));
                    }
                    catch (Exception ex)
                    {
                        
                        throw;
                    }
                    
                }
            ));
            OnActionsUpdated();
        }        

        public override bool CanGoBack => _browserView.CanGoBack;
        public override void Back()
        {
            _browserView.GoBack();
            OnActionsUpdated();
        }

        public override bool CanGoForward => _browserView.CanGoForward;
        public override void Forward()
        {
            _browserView.GoForward();
            OnActionsUpdated();
        }

        public override void Refresh()
        {
            _browserView.Refresh();
            OnActionsUpdated();
        }

        public override List<BrowserFeature> Features { get; } = new List<BrowserFeature>()
        {
            BrowserFeature.GetHtml,
            BrowserFeature.SetHtml
        };

        public override void ExecuteJavascript(string script)
        {
            throw new NotSupportedException();
        }

        public override string ExecuteJavascriptWithResult(string script)
        {
            throw new NotSupportedException();
        }

        public override string GetHtml()
        {
            return (_browserView.Document as IHTMLDocument2)?.body?.outerHTML;
        }

        public override void SetHtml(string html)
        {
            _browserView.NavigateToString(html);
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