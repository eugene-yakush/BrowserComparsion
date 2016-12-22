using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace BrowserComparsion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool _initialized = false;

        private Dictionary<TabItem, IBrowserController> _controllers;
        private IBrowserController CurrentController => _controllers?[BrowserTabCollection.SelectedItem as TabItem];

        private DotNetBrowserController dotNetBrowserController;
        private WebBrowserController webBrowserController;
        private AwesomiumController awesomiumController;
        private CefSharpController cefSharpController;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeBrowserControllers();
            InitializeBrowserPages();
        }

        private void InitializeBrowserControllers()
        {
            dotNetBrowserController = new DotNetBrowserController(DotNetBrowserView);
            webBrowserController = new WebBrowserController(WebBrowserView);
            awesomiumController = new AwesomiumController(AwesomiumView);
            cefSharpController = new CefSharpController(CefSharpView);

            dotNetBrowserController.ActionsUpdated += UpdateAvailableActions;
            webBrowserController.ActionsUpdated += UpdateAvailableActions;
            awesomiumController.ActionsUpdated += UpdateAvailableActions;
            cefSharpController.ActionsUpdated += UpdateAvailableActions;

            _controllers = new Dictionary<TabItem, IBrowserController>()
            {
                {BrowserTabCollection.Items[0] as TabItem, dotNetBrowserController},
                {BrowserTabCollection.Items[1] as TabItem, webBrowserController},
                {BrowserTabCollection.Items[2] as TabItem, awesomiumController},
                {BrowserTabCollection.Items[3] as TabItem, cefSharpController}
            };
        }

        private async Task InitializeBrowserPages()
        {
            UpdateStatusText("Initializing components...");

            foreach (KeyValuePair<TabItem, IBrowserController> controller in _controllers)
            {
                BrowserTabCollection.SelectedItem = controller.Key;
                await Task.Delay(1000);
            }

            BrowserTabCollection.SelectedItem = BrowserTabCollection.Items[0];

            _initialized = true;
            UpdateStatusText("Ready");
        }

        void UpdateStatusText(string status)
        {
            Dispatcher.Invoke(() => { StatusLabel.Content = status; });
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.Source as TabControl)?.Name == "BrowserTabCollection")
            {
                UpdateAvailableActions();
            }
        }

        private void UpdateAvailableActions()
        {
            Dispatcher.Invoke(UpdateNavigationItems);
            Dispatcher.Invoke(UpdateFeatureItems);
        }

        private void UpdateNavigationItems()
        {
            if (!_initialized)
            {
                DotNetBrowser_AddressBox.IsEnabled = false;
                DotNetBrowser_BackBtn.IsEnabled = false;
                DotNetBrowser_RefreshBtn.IsEnabled = false;
                DotNetBrowser_ForwardBtn.IsEnabled = false;

                WebBrowser_AddressBox.IsEnabled = false;
                WebBrowser_BackBtn.IsEnabled = false;
                WebBrowser_RefreshBtn.IsEnabled = false;
                WebBrowser_ForwardBtn.IsEnabled = false;

                Awesomium_AddressBox.IsEnabled = false;
                Awesomium_BackBtn.IsEnabled = false;
                Awesomium_RefreshBtn.IsEnabled = false;
                Awesomium_ForwardBtn.IsEnabled = false;

                CefSharp_AddressBox.IsEnabled = false;
                CefSharp_BackBtn.IsEnabled = false;
                CefSharp_RefreshBtn.IsEnabled = false;
                CefSharp_ForwardBtn.IsEnabled = false;
            }
            else if (CurrentController != null)
            {
                if (CurrentController is DotNetBrowserController)
                {
                    DotNetBrowser_RefreshBtn.IsEnabled = true;
                    DotNetBrowser_AddressBox.IsEnabled = true;

                    DotNetBrowser_AddressBox.Text = CurrentController.CurrentUrl;
                    DotNetBrowser_BackBtn.IsEnabled = CurrentController.CanGoBack;
                    DotNetBrowser_ForwardBtn.IsEnabled = CurrentController.CanGoForward;
                }

                if (CurrentController is WebBrowserController)
                {
                    WebBrowser_RefreshBtn.IsEnabled = true;
                    WebBrowser_AddressBox.IsEnabled = true;

                    WebBrowser_AddressBox.Text = CurrentController.CurrentUrl;
                    WebBrowser_BackBtn.IsEnabled = CurrentController.CanGoBack;
                    WebBrowser_ForwardBtn.IsEnabled = CurrentController.CanGoForward;
                }

                if (CurrentController is AwesomiumController)
                {
                    Awesomium_RefreshBtn.IsEnabled = true;
                    Awesomium_AddressBox.IsEnabled = true;

                    Awesomium_AddressBox.Text = CurrentController.CurrentUrl;
                    Awesomium_BackBtn.IsEnabled = CurrentController.CanGoBack;
                    Awesomium_ForwardBtn.IsEnabled = CurrentController.CanGoForward;
                }

                if (CurrentController is CefSharpController)
                {
                    CefSharp_RefreshBtn.IsEnabled = true;
                    CefSharp_AddressBox.IsEnabled = true;

                    CefSharp_AddressBox.Text = CurrentController.CurrentUrl;
                    CefSharp_BackBtn.IsEnabled = CurrentController.CanGoBack;
                    CefSharp_ForwardBtn.IsEnabled = CurrentController.CanGoForward;
                }
            }
        }

        private void UpdateFeatureItems()
        {
            if (!_initialized)
            {
                PagesListBox.IsEnabled = false;
                ExecuteJsBtn.IsEnabled = false;
                JsExecuteResultCheckBox.IsEnabled = false;
                GetHtmlBtn.IsEnabled = false;
                SetHtmlBtn.IsEnabled = false;
                SearchDomByIdBtn.IsEnabled = false;
                SearchDomByclassBtn.IsEnabled = false;
                SearchDomByTagBtn.IsEnabled = false;
                SetDomValueBtn.IsEnabled = false;
            }
            else
            {
                PagesListBox.IsEnabled = true;
                ExecuteJsBtn.IsEnabled = CurrentController.Features.Contains(BrowserFeature.JsExecute);
                JsExecuteResultCheckBox.IsEnabled = CurrentController.Features.Contains(BrowserFeature.JsExecuteWithResult);
                GetHtmlBtn.IsEnabled = CurrentController.Features.Contains(BrowserFeature.GetHtml);
                SetHtmlBtn.IsEnabled = CurrentController.Features.Contains(BrowserFeature.SetHtml);
                SearchDomByIdBtn.IsEnabled
                    = SearchDomByclassBtn.IsEnabled
                        = SearchDomByTagBtn.IsEnabled
                            = CurrentController.Features.Contains(BrowserFeature.GetDom);
                SetDomValueBtn.IsEnabled = CurrentController.Features.Contains(BrowserFeature.SetDom);
            }
        }

        private void DotNetBrowser_BackBtn_Click(object sender, RoutedEventArgs e)
        {
            dotNetBrowserController?.Back();
        }

        private void DotNetBrowser_RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            dotNetBrowserController?.Refresh();
        }

        private void DotNetBrowser_ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            dotNetBrowserController?.Forward();
        }

        private void WebBrowser_BackBtn_Click(object sender, RoutedEventArgs e)
        {
            webBrowserController?.Back();
        }

        private void WebBrowser_RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            webBrowserController?.Refresh();
        }

        private void WebBrowser_ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            webBrowserController?.Forward();
        }

        private void Awesomium_BackBtn_Click(object sender, RoutedEventArgs e)
        {
            awesomiumController?.Back();
        }

        private void Awesomium_RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            awesomiumController?.Refresh();
        }

        private void Awesomium_ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            awesomiumController?.Forward();
        }

        private void CefSharp_BackBtn_Click(object sender, RoutedEventArgs e)
        {
            cefSharpController?.Back();
        }

        private void CefSharp_RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            cefSharpController?.Refresh();
        }

        private void CefSharp_ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            cefSharpController?.Forward();
        }

        private void PagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string targetUrl = ((e.Source as ListBox)?.SelectedItem as ListBoxItem)?.Content?.ToString();
            if (!string.IsNullOrWhiteSpace(targetUrl) && !TrimUrlProtocol(CurrentController.CurrentUrl).Equals(TrimUrlProtocol(targetUrl), StringComparison.OrdinalIgnoreCase))
                CurrentController.Navigate(targetUrl);

            UnselectPageInBox();
        }

        private async Task UnselectPageInBox()
        {
            await Task.Delay(500);
            PagesListBox.SelectedItem = null;
        }

        private string TrimUrlProtocol(string url) => url?.Remove(0, url.IndexOf(':'));

        private void ExecuteJsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentController.Features.Contains(BrowserFeature.JsExecute))
            {
                if (JsExecuteResultCheckBox.IsChecked != null
                    && (CurrentController.Features.Contains(BrowserFeature.JsExecuteWithResult)
                        && JsExecuteResultCheckBox.IsChecked.Value))
                {
                    Dispatcher.Invoke(() => { JsExecuteResult.Text = CurrentController.ExecuteJavascriptWithResult(JsExecuteTextBox.Text); });
                }
                else
                {
                    CurrentController.ExecuteJavascript(JsExecuteTextBox.Text);
                }
            }
        }

        private void GetHtmlBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => { HtmlTextBox.Text = CurrentController.GetHtml(); });
        }

        private void SetHtmlBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentController.SetHtml(HtmlTextBox.Text);
        }

        private void SearchDomByTagBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DomSearchResultsListBox.Items.Clear();
                CurrentController.GetDomByTag(DomSearchTextBox.Text).ForEach(node => DomSearchResultsListBox.Items.Add(node));
            });
        }

        private void SearchDomByIdBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DomSearchResultsListBox.Items.Clear();
                CurrentController.GetDomById(DomSearchTextBox.Text).ForEach(node => DomSearchResultsListBox.Items.Add(node));
            });
        }

        private void SearchDomByclassBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DomSearchResultsListBox.Items.Clear();
                CurrentController.GetDomByClass(DomSearchTextBox.Text).ForEach(node => DomSearchResultsListBox.Items.Add(node));
            });
        }

        private void SetDomValueBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentController.SetDomById(DomSetElementTextBox.Text, DomSetInnerHtmlTextBox.Text);
        }

        private void ProfilerBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PerformanceWindow profilerWindow = new PerformanceWindow(_controllers.Values.ToList());
            profilerWindow.ShowDialog();
        }

        private void Expander_OnExpanded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Expander> expanders = new List<Expander>() { PagesExpander, HtmlExpander, JsExpander, DomManipulationExpander, DomSearchExpander };
                foreach (Expander expander in expanders)
                {
                    if (expander != null && expander != sender)
                    {
                        expander.IsExpanded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
