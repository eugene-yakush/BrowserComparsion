using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserComparsion
{
    public abstract class BaseWebBrowserController : IBrowserController
    {
        public event Action ActionsUpdated;
        protected ManualResetEvent loadedResetEvent = new ManualResetEvent(false);
        public virtual string CurrentUrl { get; }

        public virtual void Navigate(string url)
        {
            loadedResetEvent.Reset();
        }

        protected void Navigated()
        {
            loadedResetEvent.Set();
        }

        public void Navigate(string url, Action<int> completionPeriodCallback)
        {
            Task.Run(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Navigate(url);
                    loadedResetEvent.WaitOne();
                    sw.Stop();
                    completionPeriodCallback((int)sw.ElapsedMilliseconds);
                }
            );
        }

        public abstract bool CanGoBack { get; }
        public abstract void Back();

        public abstract bool CanGoForward { get; }
        public abstract void Forward();

        public abstract void Refresh();


        public abstract List<BrowserFeature> Features { get; }
        public abstract void ExecuteJavascript(string script);

        public abstract string ExecuteJavascriptWithResult(string script);

        public abstract string GetHtml();

        public abstract void SetHtml(string html);

        public abstract List<string> GetDomById(string id);

        public abstract List<string> GetDomByTag(string id);

        public abstract List<string> GetDomByClass(string id);

        public abstract void SetDomById(string id, string html);

        protected virtual void OnActionsUpdated()
        {
            ActionsUpdated?.Invoke();
        }
    }
}