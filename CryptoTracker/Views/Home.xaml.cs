﻿using CryptoTracker.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {
    public sealed partial class Home : Page {

        private int    limit = 60;
        private string timeSpan = "hour";

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Home() {
            this.InitializeComponent();

            TimeSpan period = TimeSpan.FromSeconds(45);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour" && this.Frame.SourcePageType.Name == "Home")
                        ALL_TimerangeButton_Click(r, null);
                });
            }, period);

            UpdateHome();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal async void UpdateHome() {

            if (LoadingControl_BTC == null)
                LoadingControl_BTC = new Loading();
            if (LoadingControl_ETH == null)
                LoadingControl_ETH = new Loading();
            if (LoadingControl_LTC == null)
                LoadingControl_LTC = new Loading();

            LoadingControl_BTC.IsLoading = true;
            LoadingControl_ETH.IsLoading = true;
            LoadingControl_LTC.IsLoading = true;

            await UpdateCoin("BTC");
            BTC_verticalAxis.Minimum = GetMinimum(App.historic);
            BTC_verticalAxis.Maximum = GetMaximum(App.historic);
            BTC_DateTimeAxis = App.AdjustAxis(BTC_DateTimeAxis, timeSpan);
            await GetStats("BTC");
            await Get24hVolume("BTC");

            await UpdateCoin("ETH");
            ETH_verticalAxis.Minimum = GetMinimum(App.historic);
            ETH_verticalAxis.Maximum = GetMaximum(App.historic);
            ETH_DateTimeAxis = App.AdjustAxis(ETH_DateTimeAxis, timeSpan);
            await GetStats("ETH");
            await Get24hVolume("ETH");

            await UpdateCoin("LTC");
            LTC_verticalAxis.Minimum = GetMinimum(App.historic);
            LTC_verticalAxis.Maximum = GetMaximum(App.historic);
            LTC_DateTimeAxis = App.AdjustAxis(LTC_DateTimeAxis, timeSpan);
            await GetStats("LTC");
            await Get24hVolume("LTC");

            await UpdateCoin("XRP");
            XRP_verticalAxis.Minimum = GetMinimum(App.historic);
            XRP_verticalAxis.Maximum = GetMaximum(App.historic);
            XRP_DateTimeAxis = App.AdjustAxis(XRP_DateTimeAxis, timeSpan);
            await GetStats("XRP");
            await Get24hVolume("XRP");
        }

        private float GetMaximum(List<PricePoint> a) {
            int i = 0;
            float max = 0;

            foreach (PricePoint type in a) {
                if (a[i].High > max)
                    max = a[i].High;

                i++;
            }

            return max;
        }
        private float GetMinimum(List<PricePoint> a) {
            int i = 0;
            float min = 50000;

            foreach (PricePoint type in a) {
                if (a[i].High < min)
                    min = a[i].High;

                i++;
            }

            return min * (float)0.993;
        }

        async public Task GetStats(String crypto) {
            await App.GetStats(crypto);

            switch (crypto) {
                case "BTC":
                    BTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    BTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    BTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    BTC_Vol24.Text = App.stats.Volume24 + "BTC";
                    break;

                case "ETH":
                    ETH_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    ETH_High.Text  = App.stats.High24   + App.coinSymbol;
                    ETH_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    ETH_Vol24.Text = App.stats.Volume24 + "ETH";
                    break;

                case "LTC":
                    LTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    LTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    LTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    LTC_Vol24.Text = App.stats.Volume24 + "LTC";
                    break;

                case "XRP":
                    XRP_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    XRP_High.Text  = App.stats.High24   + App.coinSymbol;
                    XRP_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    XRP_Vol24.Text = App.stats.Volume24 + "XRP";
                    break;

            }
        }
        async private Task Get24hVolume(String crypto) {
            await App.GetHisto(crypto, "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();

            for (int i = 0; i < App.historic.Count; i++) {
                data.Add(new App.ChartDataObject() {
                    Date   = App.historic[i].DateTime,
                    Volume = App.historic[i].Volumefrom
                });
            }

            switch (crypto) {
                case "BTC":
                    this.BTC_VolumeChart.DataContext = data;
                    break;
                case "ETH":
                    this.ETH_VolumeChart.DataContext = data;
                    break;
                case "LTC":
                    this.LTC_VolumeChart.DataContext = data;
                    break;
                case "XRP":
                    this.XRP_VolumeChart.DataContext = data;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async private Task UpdateCoin(string crypto) {
            await App.GetCurrentPrice(crypto);
            switch (crypto) {
                case "BTC":
                    BTC_curr.Text = App.BTC_now.ToString() + App.coinSymbol;
                    break;
                case "ETH":
                    ETH_curr.Text = App.ETH_now.ToString() + App.coinSymbol;
                    break;
                case "LTC":
                    LTC_curr.Text = App.LTC_now.ToString() + App.coinSymbol;
                    break;
                case "XRP":
                    XRP_curr.Text = App.XRP_now.ToString() + App.coinSymbol;
                    break;
            }

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto(crypto, "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto(crypto, "hour", limit);
                    break;

                case "year":
                    await App.GetHisto(crypto, "day", limit);
                    break;

                case "all":
                    await App.GetHisto(crypto, "day", 0);
                    break;
            }

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < App.historic.Count; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject {
                    Date   = App.historic[i].DateTime,
                    Value  = (App.historic[i].Low + App.historic[i].High) / 2,
                    Low    = App.historic[i].Low,
                    High   = App.historic[i].High,
                    Open   = App.historic[i].Open,
                    Close  = App.historic[i].Close,
                    Volume = App.historic[i].Volumefrom
                };
                data.Add(obj);

            }

            float d = 0;
            switch (crypto) {
                case "BTC":
                    d = (float)Math.Round(((App.BTC_now / App.BTC_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.BTC_change1h = d;

                    if (d < 0) {
                        BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                        d = Math.Abs(d);
                        BTC_diff.Text = "▼" + d.ToString() + "%";
                    } else {
                        BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                        BTC_diff.Text = "▲" + d.ToString() + "%";
                    }
                    break;
                case "ETH":
                    d = (float)Math.Round(((App.ETH_now / App.ETH_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.ETH_change1h = d;

                    if (d < 0) {
                        ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                        d = Math.Abs(d);
                        ETH_diff.Text = "▼" + d.ToString() + "%";
                    } else {
                        ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                        ETH_diff.Text = "▲" + d.ToString() + "%";
                    }
                    break;
                case "LTC":
                    d = (float)Math.Round(((App.LTC_now / App.LTC_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.LTC_change1h = d;

                    if (d < 0) {
                        LTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                        d = Math.Abs(d);
                        LTC_diff.Text = "▼" + d.ToString() + "%";
                    } else {
                        LTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                        LTC_diff.Text = "▲" + d.ToString() + "%";
                    }
                    break;
                case "XRP":
                    d = (float)Math.Round(((App.XRP_now / App.XRP_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.XRP_change1h = d;

                    if (d < 0) {
                        XRP_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                        d = Math.Abs(d);
                        XRP_diff.Text = "▼" + d.ToString() + "%";
                    } else {
                        XRP_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                        XRP_diff.Text = "▲" + d.ToString() + "%";
                    }
                    break;
            }

            SplineAreaSeries series = null;
            switch (crypto) {
                case "BTC":
                    series = (SplineAreaSeries)BTC_Chart.Series[0];
                    break;
                case "ETH":
                    series = (SplineAreaSeries)ETH_Chart.Series[0];
                    break;
                case "LTC":
                    series = (SplineAreaSeries)LTC_Chart.Series[0];
                    break;
                case "XRP":
                    series = (SplineAreaSeries)XRP_Chart.Series[0];
                    break;
            }

            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            switch (crypto) {
                case "BTC":
                    if (LoadingControl_BTC != null)
                        LoadingControl_BTC.IsLoading = false;
                    break;
                case "ETH":
                    if (LoadingControl_ETH != null)
                        LoadingControl_ETH.IsLoading = false;
                    break;
                case "LTC":
                    if (LoadingControl_LTC != null)
                        LoadingControl_LTC.IsLoading = false;
                    break;
                case "XRP":
                    if (LoadingControl_XRP != null)
                        LoadingControl_XRP.IsLoading = false;
                    break;

            }

        }





        private void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateHome();
        }        

        private void Tapped_BTC(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(CoinDetails), "BTC");
        }
        private void Tapped_ETH(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(CoinDetails), "ETH");
        }
        private void Tapped_LTC(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(CoinDetails), "LTC");
        }
        private void Tapped_XRP(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(CoinDetails), "XRP");
        }
    }
}
