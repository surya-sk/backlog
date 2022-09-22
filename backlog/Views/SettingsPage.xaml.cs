﻿using System;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using backlog.Utils;
using Windows.ApplicationModel.Email;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using backlog.Saving;
using backlog.Logging;
using System.Text;
using backlog.Auth;
using System.Windows.Input;
using Windows.UI.Xaml.Controls.Primitives;
using MvvmHelpers.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel.Channels;
using backlog.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace backlog.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        bool signedIn;
        static string MIT_LICENSE = "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: \n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. \n\nTHE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
        static string GNU_LICENSE = "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. \n\nYou should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/";
        static string CHANGE_LOG = "\u2022 Film, TV and game backlogs now have a button to play the trailer.\n" +
            "\u2022 There is now a button that opens web results for the backlog.\n" +
            "\u2022 Today's date is now the minimum date when picking target date.\n" +
            "\u2022 The homepage now shows upcoming backlogs.\n" +
            "\u2022 The app can now show upcoming backlogs in the live tile.\n";
        string ChangelogTitle = "New this version - 30 July, 2022";
        public string Version = Settings.Version;


        public SettingsViewModel ViewModel { get; set; } = new SettingsViewModel();

        public SettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            ViewModel.NavigateToMainPageFunc = NavigateToMainPage;

            MyLicense.Text = GNU_LICENSE;
            WCTLicense.Text = MIT_LICENSE;
            WinUILicense.Text = MIT_LICENSE;
            NewtonsoftLicense.Text = MIT_LICENSE;
            Changelog.Text = CHANGE_LOG;
            TileContentButtons.SelectedValue = Settings.TileContent;
            // show back button
            var view = SystemNavigationManager.GetForCurrentView();
            view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            view.BackRequested += View_BackRequested;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            signedIn = Settings.IsSignedIn;
            if(e.Parameter != null)
            {
                mainPivot.SelectedIndex = (int)e.Parameter;
            }
            if(signedIn)
            {
                AccountPanel.Visibility = Visibility.Visible;
                await SetUserPhotoAsync();
                SignInText.Visibility = Visibility.Collapsed;
                WarningText.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Show the user photo
        /// </summary>
        /// <returns></returns>
        private async Task SetUserPhotoAsync()
        {
            string userName = ApplicationData.Current.LocalSettings.Values["UserName"]?.ToString();
            UserNameText.Text = $"Hey there, {userName}! You are all synced.";
            var cacheFolder = ApplicationData.Current.LocalCacheFolder;
            try
            {
                var accountPicFile = await cacheFolder.GetFileAsync("profile.png");
                using (IRandomAccessStream stream = await accountPicFile.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage image = new BitmapImage();
                    stream.Seek(0);
                    await image.SetSourceAsync(stream);
                    AccountPic.ProfilePicture = image;
                }
            }
            catch
            {
                // No image set
            }
        }

        /// <summary>
        /// Go back if possible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }

            e.Handled = true;
        }

        private void NavigateToMainPage()
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
