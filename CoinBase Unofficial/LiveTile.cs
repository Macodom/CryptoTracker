﻿using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace CoinBase {
    class LiveTile {

        internal void SendStockTileNotification(string symbol, double price, double diff, DateTime dateUpdated) {

            XmlDocument content = GenerateNotificationContent(symbol, price, diff, dateUpdated);

            TileNotification notification = new TileNotification(content);

            notification.Tag = symbol;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private XmlDocument GenerateNotificationContent(string symbol, double price, double diff, DateTime dateUpdated) {
            string percentString = (diff < 0 ? "▼" : "▲") + " " + Math.Abs(diff).ToString("N") + "%";

            var content = new TileContent() {
                Visual = new TileVisual() {
                    TileMedium = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText(){
                                    Text = symbol
                                },

                                new AdaptiveText(){
                                    Text = (App.coin.Equals("EUR") ) ? price.ToString("N") +"€" : price.ToString("N") +"$"
                                },

                                new AdaptiveText(){
                                    Text = percentString,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText(){
                                    Text = dateUpdated.ToString("t"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };

            return content.GetXml();
        }

        public void UpdateLiveTile() {

            try {
                LiveTile l = new LiveTile();
                l.SendStockTileNotification("BTC", App.BTC_now, App.BTC_change1h, DateTime.Now);
                l.SendStockTileNotification("ETH", App.ETH_now, App.ETH_change1h, DateTime.Now);
                l.SendStockTileNotification("LTC", App.LTC_now, App.LTC_change1h, DateTime.Now);

            } catch (Exception ex) {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

    }
}
