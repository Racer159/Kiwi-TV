﻿using Kiwi_TV.API.Twitch;
using Kiwi_TV.API.Twitch.Models;
using Kiwi_TV.Helpers;
using Kiwi_TV.Models;
using Kiwi_TV.Views.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views.ChannelSources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Twitch : Page
    {
        DeviceFormFactorType DeviceType;
        ObservableCollection<string> categories = new ObservableCollection<string>();
        ObservableCollection<string> languages = new ObservableCollection<string>();
        TwitchViewModel _viewModel;

        public Twitch()
        {
            this.InitializeComponent();
            CategoryBox.ItemsSource = categories;
            DeviceType = UWPHelper.GetDeviceFormFactorType();

            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TitleImage.Margin = new Thickness(48, 0, 0, 0);
                GridViewIconSize.Tag = 115;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TwitchViewModel)
            {
                _viewModel = (TwitchViewModel)e.Parameter;
                this.DataContext = _viewModel;

                if (this._viewModel.SearchChannels != null && this._viewModel.SearchChannels.Length > 0)
                {
                    CategoryWrap.Visibility = Visibility.Visible;
                    LiveWrap.Visibility = Visibility.Collapsed;
                }
                else if (this._viewModel.LiveChannels != null && this._viewModel.LiveChannels.Length > 0)
                {
                    CategoryWrap.Visibility = Visibility.Visible;
                    SearchWrap.Visibility = Visibility.Collapsed;
                }
                else
                {
                    await GetLiveNow();
                }
            }
            else
            {
                _viewModel = new TwitchViewModel();
                this.DataContext = _viewModel;
            }
        }

        private void CategoryBox_GotFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            categories.Add("News");
            categories.Add("Science/Technology");
            categories.Add("Entertainment");
            categories.Add("Sports");
            categories.Add("Gaming");
            categories.Add("Other");
        }

        private void CategoryBox_LostFocus(object sender, RoutedEventArgs e)
        {
            categories.Clear();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible && SearchChannelsGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)SearchChannelsGridView.SelectedItem;
                await ChannelManager.AddChannel(GenerateTwitchChannel(selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + selected.DisplayName).ShowAsync();
            }
            else if (LiveWrap.Visibility == Visibility.Visible && LiveNowGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)LiveNowGridView.SelectedItem;
                await ChannelManager.AddChannel(GenerateTwitchChannel(selected));
                await new Windows.UI.Popups.MessageDialog("Successfully added " + selected.DisplayName).ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a Twitch.tv channel to add.").ShowAsync();
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible && SearchChannelsGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)SearchChannelsGridView.SelectedItem;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateTwitchChannel(selected), ""));
            }
            else if (LiveWrap.Visibility == Visibility.Visible && LiveNowGridView.SelectedItem is TwitchChannel)
            {
                TwitchChannel selected = (TwitchChannel)LiveNowGridView.SelectedItem;
                Frame.Navigate(typeof(Views.Player), new Tuple<Channel, object>(GenerateTwitchChannel(selected), ""));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please select a Twitch.tv channel to test.").ShowAsync();
            }
        }

        private Channel GenerateTwitchChannel(TwitchChannel selected)
        {
            List<string> languages = new List<string>();
            languages.Add(TwitchAPI.ConvertLanguageCode(selected.Language));
            return new Channel(selected.DisplayName, selected.Logo, "http://usher.ttvnw.net/api/channel/hls/" + selected.Name + ".m3u8", languages, false, CategoryBox.Text, "twitch", true);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await RunSearch();
        }

        private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await RunSearch();
            }
        }

        private void SearchChannelsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedSearch = (TwitchChannel)SearchChannelsGridView.SelectedItem;
        }

        private void LiveNowGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchWrap.Visibility == Visibility.Visible)
            {
                Frame.BackStack.Add(new PageStackEntry(typeof(Views.ChannelSources.Twitch), new TwitchViewModel(), new DrillInNavigationTransitionInfo()));
                CategoryWrap.Visibility = Visibility.Visible;
                SearchWrap.Visibility = Visibility.Collapsed;
            }
        }

        private async Task RunSearch()
        {
            _viewModel.SearchChannels = new TwitchChannel[0];

            if (SearchBox.Text != "")
            {
                if (LiveWrap.Visibility == Visibility.Visible)
                {
                    Frame.BackStack.Add(new PageStackEntry(typeof(Views.ChannelSources.Twitch), new TwitchViewModel(), new DrillInNavigationTransitionInfo()));
                    CategoryWrap.Visibility = Visibility.Visible;
                    LiveWrap.Visibility = Visibility.Collapsed;
                }

                SearchLoadingSpinner.Visibility = Visibility.Visible;
                TwitchSearchResults results = await TwitchAPI.RetrieveSearchResults(Uri.EscapeDataString(SearchBox.Text));
                SearchLoadingSpinner.Visibility = Visibility.Collapsed;
                _viewModel.SearchChannels = results.Channels;
                if (SearchChannelsGridView.Items.Count > 0)
                {
                    SearchChannelsGridView.SelectedIndex = 0;
                }
            }
        }

        private async Task GetLiveNow()
        {
            _viewModel.LiveChannels = new TwitchChannel[0];
            LiveLoadingSpinner.Visibility = Visibility.Visible;
            TwitchSearchResults results = await TwitchAPI.RetrieveLiveStreams();
            LiveLoadingSpinner.Visibility = Visibility.Collapsed;
            _viewModel.LiveChannels = results.Channels;
        }
    }
}