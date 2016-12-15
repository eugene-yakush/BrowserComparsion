using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrowserComparsion
{
    public interface IBrowserController
    {
        event Action ActionsUpdated;

        string CurrentUrl { get; }

        void Navigate(string url);

        bool CanGoBack { get; }
        void Back();

        bool CanGoForward { get; }
        void Forward();

        void Refresh();        

        List<BrowserFeature> Features { get; }


        void ExecuteJavascript(string script);

        string ExecuteJavascriptWithResult(string script);

        string GetHtml();
        void SetHtml(string html);

        List<string> GetDomById(string id);
        List<string> GetDomByTag(string id);
        List<string> GetDomByClass(string id);

        void SetDomById(string id, string html);
    }

    public enum BrowserFeature
    {
        JsExecute,
        JsExecuteWithResult,
        GetHtml,
        SetHtml,
        GetDom,
        SetDom
    }
}