﻿using Kiwi_TV.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kiwi_TV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartTutorial : Page
    {
        DeviceFormFactorType DeviceType;
        int currentPage = 0;

        public StartTutorial()
        {
            this.InitializeComponent();
            DeviceType = UWPHelper.GetDeviceFormFactorType();
            if (DeviceType == DeviceFormFactorType.Phone)
            {
                TutorialWrap.Margin = new Thickness(20, 20, 20, 20);
                SubText.Margin = new Thickness(10, 220, 10, 10);
                TitleText.FontSize = 20;
                SubText.FontSize = 14;
                ExampleImage.Height = 150;
            }
            else if (DeviceType == DeviceFormFactorType.Xbox)
            {
                ExampleImage.Margin = new Thickness(10, -100, 10, 10);
                TitleText.Margin = new Thickness(10, 100, 10, 10);
                SubText.Margin = new Thickness(10, 170, 10, 10);
                ButtonWrap.Margin = new Thickness(10, 250, 10, 10);
                TitleText.FontSize = 20;
                SubText.FontSize = 14;
                ExampleImage.Height = 150;
                GreenWrap.Margin = new Thickness(-48, -75, -48, -27);
            }

            ChannelManager.MigrateChannelList();
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Channels), false);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            if (currentPage == 1 && DeviceType != DeviceFormFactorType.Xbox)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Favorite.png"));
                TitleText.Text = "Star your Favorite Channels";
                SubText.Text = "Select the star in the upper right corner.";
            }
            else if (currentPage == 1 && DeviceType == DeviceFormFactorType.Xbox)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Favorite.png"));
                TitleText.Text = "Star your Favorite Channels";
                SubText.Text = "Press Y to add to favorites.";
            }
            else if (currentPage == 2)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Live.png"));
                TitleText.Text = "See which Channels are Live";
                SubText.Text = "Look for the red marker on channel tiles.";
            }
            else if (currentPage == 3)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Add.png"));
                TitleText.Text = "Add your own Channels";
                SubText.Text = "Add channels from Twitch, UStream, and more.";
            }
            else if (currentPage == 4)
            {
                ExampleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Welcome.png"));
                TitleText.Text = "Lets Get Started";
                SubText.Text = "You're all set to begin.";
                Next.Content = "Start";
            }
            else if (currentPage == 5)
            {
                Frame.Navigate(typeof(Views.Channels), false);
            }
        }

        private void Next_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceType == DeviceFormFactorType.Xbox)
            {
                Next.Focus(FocusState.Keyboard);
            }
        }
    }
}
