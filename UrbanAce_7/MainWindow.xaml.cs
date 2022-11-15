﻿using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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
using System.Runtime.InteropServices;

namespace UrbanAce_7
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        [DllImport("user32.dll")] 
        private static extern short GetKeyState(int nVirtKey);

        public static readonly string MainTitle = "Urban Ace";

        //Setting : 0  Full : 1  WithContents:2
        private int displayMode = 2;

        private bool isCtrlPressed => GetKeyState(0xA2) < 0 || GetKeyState(0xA3) < 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await deleteWebViewData();
            try
            {
                if (WithContent.INSTANCE == null || WithContent.INSTANCE.Content != null) return;
                WithContent.INSTANCE.webView.Dispose();
            } catch{}
        }

        private async Task deleteWebViewData()
        {
            if (!WithContent.isInstanceCreated) return;
            WebView2 webview = WithContent.INSTANCE.webView;
            try
            {
                if (webview == null || webview.CoreWebView2 is null) return;

                var p = webview.CoreWebView2.Profile;
                await p.ClearBrowsingDataAsync();
            } catch (ObjectDisposedException de)
            {
                Console.WriteLine($"Webview is Disposed : Msg : {de}");
            }
        }

        private async void NavigationWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && isCtrlPressed)
            {
                string a = Clipboard.GetText().Replace(Environment.NewLine, "");
                WebView2 web = WithContent.INSTANCE.webView;
                if (web == null) return;
                if (a.Contains("https://"))
                {
                    await WithContent.INSTANCE.setWebView(a);
                } else if (a.Length == 11)
                {
                    await WithContent.INSTANCE.setYoutubeEnbedContent(a);
                }
            }
            if (e.Key == Key.F && isCtrlPressed)
            {
                displayMode = displayMode == 0 ? 2 : 0;
                Title = displayMode == 0 ? $"{MainTitle} -- 設定" : MainTitle;
                switch (displayMode) 
                {
                    case 0:
                        WithContent.INSTANCE.FloorName.Focus();
                        WithContent.INSTANCE.webView.Dispose();
                        WithContent.INSTANCE.webView = null;
                        var c = new Setting();
                        NavigationService.Navigate(c);
                        ResizeMode = ResizeMode.CanResize;
                        break;
                    case 2:
                        WindowState = WindowState.Normal;
                        Width = 500;
                        Height = 600;
                        ResizeMode = ResizeMode.CanMinimize;
                        var wc = new WithContent();
                        NavigationService.Navigate(wc);
                        break;
                }
            }
            if (e.Key == Key.K)
            {
                ElevatorDirection d = WithContent.INSTANCE.direction;
                WithContent.INSTANCE.UpdateArrow(d == ElevatorDirection.UP ? ElevatorDirection.DOWN :
                    ElevatorDirection.UP);
            }
            if (e.Key == Key.L)
            {
                WithContent.INSTANCE.UpdateArrow(ElevatorDirection.NONE);
            }
            if (e.Key == Key.M)
            {
                WithContent.INSTANCE.DoArrowAnim();
            }
            if (e.Key == Key.Escape) this.Close();
        }
    }
}