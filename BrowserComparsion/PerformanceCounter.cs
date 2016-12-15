using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserComparsion
{
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