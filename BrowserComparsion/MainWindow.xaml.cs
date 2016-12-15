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
            DotNetBrowserController dotNetBrowserController = new DotNetBrowserController(DotNetBrowserView);
            WebBrowserController webBrowserController = new WebBrowserController(WebBrowserView);
            AwesomiumController awesomiumController = new AwesomiumController(AwesomiumView);
            CefSharpController cefSharpController = new CefSharpController(CefSharpView);

            dotNetBrowserController.ActionsUpdated += UpdateAvailableActions;
            webBrowserController.ActionsUpdated += UpdateAvailableActions;
            awesomiumController.ActionsUpdated += UpdateAvailableActions;
            cefSharpController.ActionsUpdated += UpdateAvailableActions;

            _controllers = new Dictionary<TabItem, IBrowserController>()
            {
                {DotNetBrowserView.Parent as TabItem, dotNetBrowserController },
                {WebBrowserView.Parent as TabItem,  webBrowserController},
                {AwesomiumView.Parent as TabItem, awesomiumController },
                {CefSharpView.Parent as TabItem, cefSharpController }
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
                AddressBox.IsEnabled = false;
                BackBtn.IsEnabled = false;
                RefreshBtn.IsEnabled = false;
                ForwardBtn.IsEnabled = false;
                NavigateBtn.IsEnabled = false;
            }
            else if (CurrentController != null)
            {
                RefreshBtn.IsEnabled = true;
                NavigateBtn.IsEnabled = true;
                AddressBox.IsEnabled = true;

                AddressBox.Text = CurrentController.CurrentUrl;
                BackBtn.IsEnabled = CurrentController.CanGoBack;
                ForwardBtn.IsEnabled = CurrentController.CanGoForward;
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

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentController?.Back();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentController?.Refresh();
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentController?.Forward();
        }

        private void NavigateBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentController?.Navigate(AddressBox.Text);
        }

        private void PagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string targetUrl = ((e.Source as ListBox)?.SelectedItem as ListBoxItem)?.Content?.ToString();
            if(!TrimUrlProtocol(CurrentController.CurrentUrl).Equals(TrimUrlProtocol(targetUrl), StringComparison.OrdinalIgnoreCase))
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
        }
    }
}
