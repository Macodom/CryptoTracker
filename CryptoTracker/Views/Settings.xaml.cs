﻿using Microsoft.AppCenter.Analytics;
using System;
using System.Reflection;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CryptoTracker {
    public sealed partial class Settings : Page {

        private string v;

        public Settings() {
            this.InitializeComponent();

            var assembly = typeof(App).GetTypeInfo().Assembly;
            v = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            VersionTextBlock.Text = "Version: " + v;

            ThemeSwitcher.IsOn = App.localSettings.Values["Theme"].Equals("Dark");

            switch (App.localSettings.Values["Coin"]) {
                case "EUR":
                    EUR.IsSelected = true;
                    break;
                case "USD":
                    USD.IsSelected = true;
                    break;
                case "GBP":
                    GBP.IsSelected = true;
                    break;
                case "CAD":
                    CAD.IsSelected = true;
                    break;
                case "AUD":
                    AUD.IsSelected = true;
                    break;
                case "MXN":
                    MXN.IsSelected = true;
                    break;
                case "CNY":
                    CNY.IsSelected = true;
                    break;
                case "JPY":
                    JPY.IsSelected = true;
                    break;
                case "INR":
                    INR.IsSelected = true;
                    break;
            }
            CoinComboBox.PlaceholderText = App.localSettings.Values["Coin"].ToString();

            //Show feedback button
            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported()) {
                this.feedbackButton.IsEnabled = true;
            } else {
                this.feedbackButton.IsEnabled = false;
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void ThemeToogled(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            Analytics.TrackEvent("ThemeToogled");
            if (toggleSwitch != null) {
                if (toggleSwitch.IsOn == true) {
                    App.localSettings.Values["Theme"] = "Dark";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                } else {
                    App.localSettings.Values["Theme"] = "Light";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                }
            }
        }

        private async void feedbackButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("feedbackButton_Click");
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        private async void reviewButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("reviewButton_Click");
            await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store:reviewapp?appid=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }
        private async void mailButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("mailButton_Click");
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("ismael.em@outlook.com"));
            emailMessage.Subject = "Feedback for CoinBase Unofficial v" + v;

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            switch (((ComboBoxItem)c.SelectedItem).Name.ToString()) {
                case "EUR":
                    App.localSettings.Values["Coin"] = "EUR";
                    App.coin       = "EUR";
                    App.coinSymbol = "€";
                    break;
                case "USD":
                    App.localSettings.Values["Coin"] = "USD";
                    App.coin = "USD";
                    App.coinSymbol = "$";
                    break;
                case "GBP":
                    App.localSettings.Values["Coin"] = "GBP";
                    App.coin = "GBP";
                    App.coinSymbol = "£";
                    break;
                case "CAD":
                    App.localSettings.Values["Coin"] = "CAD";
                    App.coin = "CAD";
                    App.coinSymbol = "$";
                    break;
                case "AUD":
                    App.localSettings.Values["Coin"] = "AUD";
                    App.coin = "AUD";
                    App.coinSymbol = "$";
                    break;
                case "MXN":
                    App.localSettings.Values["Coin"] = "MXN";
                    App.coin       = "MXN";
                    App.coinSymbol = "$";
                    break;
                case "CNY":
                    App.localSettings.Values["Coin"] = "CNY";
                    App.coin = "CNY";
                    App.coinSymbol = "¥";
                    break;
                case "JPY":
                    App.localSettings.Values["Coin"] = "JPY";
                    App.coin = "JPY";
                    App.coinSymbol = "¥";
                    break;
                case "INR":
                    App.localSettings.Values["Coin"] = "INR";
                    App.coin = "INR";
                    App.coinSymbol = "₹";
                    break;
            }
        }
    }
}
