using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_TF2ItemList.Model;
using Project_TF2ItemList.Repository;
using Project_TF2ItemList.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_TF2ItemList.ViewModel
{
    public class MainWindowVM : ObservableObject
    {
        private Page _currentPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private string _switchPageButtonText = "INSPECT ITEM";
        public string SwitchPageButtonText
        {
            get { return _switchPageButtonText; }
            set
            {
                _switchPageButtonText = value;
                OnPropertyChanged(SwitchPageButtonText);
            }
        }

        public ItemOverview OverviewPage { get; private set; } = new ItemOverview();
        public ItemInspection InspectionPage { get; private set; } = new ItemInspection();

        public RelayCommand SwitchPageCommand { get; private set; }

        public MainWindowVM()
        {
            // Set the initial page
            CurrentPage = OverviewPage;

            // Initialize commands
            SwitchPageCommand = new RelayCommand(SwitchPage, CanSwitchPage);
            (OverviewPage.DataContext as ItemOverviewVM).PropertyChanged += ReloadSwitchPageButton;
        }

        private void ReloadSwitchPageButton(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SwitchPageCommand.NotifyCanExecuteChanged();
        }

        private void SwitchPage()
        {
            // check the current visible page type
            if (CurrentPage is ItemOverview)
            {
                // Get the selected pokemon
                Item selectedItem = (OverviewPage.DataContext as ItemOverviewVM).SelectedItem;
                if (selectedItem == null) return;

                // Set the CurrentPokemon of the DetailPageVM to be the selected pokemon
                (InspectionPage.DataContext as ItemInspectionVM).CurrentItem = selectedItem;

                // Set the current page
                CurrentPage = InspectionPage;

                // Set the button text
                SwitchPageButtonText = "GO BACK";
            }
            else
            {
                // Set the current page
                CurrentPage = OverviewPage;

                // Set the button text
                SwitchPageButtonText = "INSPECT ITEM";
            }
        }

        private bool CanSwitchPage()
        {
            return (OverviewPage.DataContext as ItemOverviewVM).SelectedItem != null;
        }
    }
}
