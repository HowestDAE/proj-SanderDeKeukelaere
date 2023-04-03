using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_TF2ItemList.Model;
using Project_TF2ItemList.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.ViewModel
{
    public class ItemOverviewVM : ObservableObject
    {
        static public IItemRepository ItemRepository { get; set; }
        private APIItemRepository _apiRepository = new APIItemRepository();
        private LocalItemRepository _localRepository = new LocalItemRepository();

        private List<Item> _items;
        public List<Item> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public List<string> Classes
        {
            get { return classes; }
            private set
            {
                classes = value;
                OnPropertyChanged(nameof(Classes));
            }
        }

        private List<string> classes;
        private Item selectedItem;

        public string SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                _selectedClass = value;
                GetItems(_selectedClass);
                OnPropertyChanged(nameof(SelectedClass));
            }
        }

        private string _selectedClass;
        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private bool _isRepositoryAPI = true;

        private string _repositoryButtonText = "SWITCH TO OFFLINE REPOSITORY";
        public string RepositoryButtonText
        {
            get { return _repositoryButtonText; }
            private set
            {
                _repositoryButtonText = value;
                OnPropertyChanged(nameof(RepositoryButtonText));
            }
        }
        public RelayCommand SwitchRepositoryCommand { get; private set; }

        public ItemOverviewVM()
        {
            ItemRepository = _apiRepository;
            LoadItemsAndClasses();

            SwitchRepositoryCommand = new RelayCommand(SwitchRepository);
        }

        private async void LoadItemsAndClasses()
        {
            Items = await ItemRepository.GetItems();

            // The classes only need to be set once (otherwise "all classes" will start to be added multiple times)
            if (Classes == null)
            {
                List<string> classes = await ItemRepository.GetClasses();

                // Add "all classes" to classes list
                classes.Add("<all classes>");
                SelectedClass = classes[classes.Count - 1];

                Classes = classes;
            }

            // Sort items
            GetItems(SelectedClass);
        }

        private async void GetItems(string className)
        {
            if (_selectedClass == null) return;

            if (_selectedClass.Equals("<all classes>"))
            {
                Items = await ItemRepository.GetItems();
            }
            else
            {
                Items = await ItemRepository.GetItems(_selectedClass);
            }
        }

        private void SwitchRepository()
        {
            _isRepositoryAPI = !_isRepositoryAPI;
            if (_isRepositoryAPI)
            {
                ItemRepository = _apiRepository;
            }
            else
            {
                ItemRepository = _localRepository;
            }
            RepositoryButtonText = _isRepositoryAPI ? "SWITCH TO OFFLINE REPOSITORY" : "SWITCH TO API REPOSITORY";
            LoadItemsAndClasses();
        }
    }
}
