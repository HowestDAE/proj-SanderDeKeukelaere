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

        private List<string> _classes;
        public List<string> Classes
        {
            get { return _classes; }
            private set
            {
                _classes = value;
                OnPropertyChanged(nameof(Classes));
            }
        }

        private List<string> _itemTypes;
        public List<string> ItemTypes
        {
            get { return _itemTypes; }
            private set
            {
                _itemTypes = value;
                OnPropertyChanged(nameof(ItemTypes));
            }
        }

        private List<string> _itemSlots;
        public List<string> ItemSlots
        {
            get { return _itemSlots; }
            private set
            {
                _itemSlots = value;
                OnPropertyChanged(nameof(ItemSlots));
            }
        }

        private string _selectedClass;
        public string SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                _selectedClass = value;
                RefreshItemFiltering();
                OnPropertyChanged(nameof(SelectedClass));
            }
        }

        private string _selectedItemType;
        public string SelectedItemType
        {
            get { return _selectedItemType; }
            set
            {
                _selectedItemType = value;
                RefreshItemFiltering();
                OnPropertyChanged(nameof(SelectedItemType));
            }
        }

        private string _selectedItemSlot;
        public string SelectedItemSlot
        {
            get { return _selectedItemSlot; }
            set
            {
                _selectedItemSlot = value;
                RefreshItemFiltering();
                OnPropertyChanged(nameof(SelectedItemSlot));
            }
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
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

        private async void RefreshItemFiltering()
        {
            Items = await ItemRepository.GetItems();
            GetItemsByClass();
            GetItemsByType();
            GetItemsBySlot();
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

            List<string> itemTypes = await ItemRepository.GetItemTypes();
            // Add "all classes" to itemtypes list
            itemTypes.Add("<all item types>");
            SelectedItemType = itemTypes[itemTypes.Count - 1];
            ItemTypes = itemTypes;


            List<string> itemSlots = await ItemRepository.GetItemSlots();
            // Add "all classes" to itemslots list
            itemSlots.Add("<all item slots>");
            SelectedItemSlot = itemSlots[itemSlots.Count - 1];
            ItemSlots = itemSlots;

            // Sort items
            RefreshItemFiltering();
        }

        private void GetItemsByClass()
        {
            if (_selectedClass == null) return;

            if (!_selectedClass.Equals("<all classes>"))
            {
                List<Item> items = new List<Item>();

                foreach (Item item in _items)
                {
                    if (item.Classes == null) continue;

                    if (item.Classes.Contains(_selectedClass)) items.Add(item);
                }

                Items = items;
            }
        }

        private void GetItemsByType()
        {
            if (_selectedItemType == null) return;

            if (!_selectedItemType.Equals("<all item types>"))
            {
                List<Item> items = new List<Item>();

                foreach (Item item in _items)
                {
                    if (item.ItemType == null) continue;

                    if (item.ItemType.Equals(_selectedItemType)) items.Add(item);
                }

                Items = items;
            }
        }

        private void GetItemsBySlot()
        {
            if (_selectedItemSlot == null) return;

            if (!_selectedItemSlot.Equals("<all item slots>"))
            {
                List<Item> items = new List<Item>();

                foreach (Item item in _items)
                {
                    if (item.ItemSlot == null) continue;

                    if (item.ItemSlot.Equals(_selectedItemSlot)) items.Add(item);
                }

                Items = items;
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
