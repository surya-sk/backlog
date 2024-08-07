﻿using Backlogs.Models;
using Backlogs.Services;
using Backlogs.Utils.UWP;
using Backlogs.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Backlogs.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BacklogsPage : Page
    {
        int backlogIndex = -1;
        bool sync = false;

        public IncrementalLoadingCollection<BacklogSource, Backlog> IncompleteBacklogs { get; set; }
        public IncrementalLoadingCollection<BacklogSource, Backlog> FilmBacklogs { get; set; }
        public IncrementalLoadingCollection<BacklogSource, Backlog> TvBacklogs { get; set; }
        public IncrementalLoadingCollection<BacklogSource, Backlog> GameBacklogs { get; set; }
        public IncrementalLoadingCollection<BacklogSource, Backlog> MusicBacklogs { get; set; }
        public IncrementalLoadingCollection<BacklogSource, Backlog> BookBacklogs { get; set; }

        public BacklogsViewModel ViewModel { get; set; } 

        public BacklogsPage()
        {
            this.InitializeComponent();
            ViewModel = new BacklogsViewModel(App.Services.GetRequiredService<INavigation>(), App.Services.GetRequiredService<IDialogHandler>(),
                App.Services.GetRequiredService<IFileHandler>(), App.Services.GetService<IUserSettings>());
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null && e.Parameter.ToString() != "")
            {
                if (e.Parameter.ToString() == "sync")
                {
                    sync = true;
                }
                else
                {
                    // for backward connected animation
                    backlogIndex = int.Parse(e.Parameter.ToString());
                }
            }
            await ViewModel.SyncBacklogs(sync);
            IncompleteBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.IncompleteBacklogs));
            FilmBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.FilmBacklogs));
            TvBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.TvBacklogs));
            MusicBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.MusicBacklogs));
            GameBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.GameBacklogs));
            BookBacklogs = new IncrementalLoadingCollection<BacklogSource, Backlog>(new BacklogSource(ViewModel.BookBacklogs));
            var view = SystemNavigationManager.GetForCurrentView();
            view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            view.BackRequested += View_BackRequested;
        }

        private void View_BackRequested(object sender, BackRequestedEventArgs e)
        {
            ViewModel.GoBack();
            e.Handled = true;
        }


        /// <summary>
        /// Opens the Backlog details page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BacklogView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedBacklog = (Backlog)e.ClickedItem;
            PivotItem pivotItem = (PivotItem)mainPivot.SelectedItem;
            // Prepare connected animation based on which section the user is on
            switch (pivotItem.Header.ToString())
            {
                default:
                    BacklogsGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
                case "films":
                    FilmsGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
                case "tv":
                    TVGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
                case "books":
                    BooksGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
                case "games":
                    GamesGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
                case "albums":
                    AlbumsGrid.PrepareConnectedAnimation("cover", selectedBacklog, "coverImage");
                    break;
            }
            Frame.Navigate(typeof(BacklogPage), selectedBacklog.id, new SuppressNavigationTransitionInfo());
        }


        /// <summary>
        /// Finish connected animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BacklogsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (backlogIndex != -1)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("backAnimation");
                try
                {
                    await BacklogsGrid.TryStartConnectedAnimationAsync(animation, ViewModel.Backlogs[backlogIndex], "coverImage");
                }
                catch
                {
                    // : )
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await SearchDialog.ShowAsync();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<string> suggestions = new List<string>();
                var splitText = sender.Text.ToLower().Split(' ');
                ObservableCollection<Backlog> backlogsToSearch = null;
                switch (mainPivot.SelectedIndex)
                {
                    case 0:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs);
                        break;
                    case 1:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs.Where(b => b.Type == "Film"));
                        break;
                    case 2:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs.Where(b => b.Type == "Album"));
                        break;
                    case 3:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs.Where(b => b.Type == "TV"));
                        break;
                    case 4:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs.Where(b => b.Type == "Game"));
                        break;
                    case 5:
                        backlogsToSearch = new ObservableCollection<Backlog>(ViewModel.IncompleteBacklogs.Where(b => b.Type == "Book"));
                        break;
                }
                foreach (var backlog in backlogsToSearch)
                {
                    var found = splitText.All((key) =>
                    {
                        return backlog.Name.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suggestions.Add(backlog.Name);
                    }
                }
                if (suggestions.Count == 0)
                {
                    suggestions.Add("No results found");
                }
                sender.ItemsSource = suggestions;

            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var selectedBacklog = ViewModel.IncompleteBacklogs.FirstOrDefault(b => b.Name == args.ChosenSuggestion.ToString());
            SearchDialog.Hide();
            Frame.Navigate(typeof(BacklogPage), selectedBacklog.id, null);
        }
    }
}
