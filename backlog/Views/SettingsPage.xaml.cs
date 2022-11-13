﻿using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Backlogs.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Backlogs.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            ViewModel = new SettingsViewModel(App.GetNavigationService());
            // show back button
            var view = SystemNavigationManager.GetForCurrentView();
            view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            view.BackRequested += ViewModel.GoBack;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                mainPivot.SelectedIndex = (int)e.Parameter;
            }
            if(ViewModel.SignedIn)
            {
                await ViewModel.SetUserPhotoAsync();
            }
            base.OnNavigatedTo(e);
        }
    }
}
