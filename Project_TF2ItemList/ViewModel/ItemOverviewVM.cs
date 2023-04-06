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

        private string _selectedClass = "<all classes>";
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

        private string _selectedItemType = "<all item types>";
        public string SelectedItemType
        {
            get { return _selectedItemType; }
            set
            {
                _selectedItemType = value;
                RefreshItemTypeFiltering();
                OnPropertyChanged(nameof(SelectedItemType));
            }
        }

        private string _selectedItemSlot = "<all item slots>";
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
            List<Item> itemsCopy = new List<Item>(await ItemRepository.GetItems());

            if (!NeedsBigItemLibrary())
            {
                itemsCopy.RemoveRange(200, itemsCopy.Count() - 200);
            }

            GetItemsByClass(itemsCopy);
            GetItemsBySlot(itemsCopy);

            ReloadItemTypes(itemsCopy);
            GetItemsByType(itemsCopy);

            Items = itemsCopy;
        }

        private async void RefreshItemTypeFiltering()
        {
            List<Item> itemsCopy = new List<Item>(await ItemRepository.GetItems());

            if (!NeedsBigItemLibrary())
            {
                itemsCopy.RemoveRange(200, itemsCopy.Count() - 200);
            }

            GetItemsByClass(itemsCopy);
            GetItemsBySlot(itemsCopy);
            GetItemsByType(itemsCopy);

            Items = itemsCopy;
        }

        private async void LoadItemsAndClasses()
        {
            List<Item> itemsCopy = new List<Item>(await ItemRepository.GetItems());
            itemsCopy.RemoveRange(200, itemsCopy.Count() - 200);
            GetItemsByClass(itemsCopy);
            Items = itemsCopy;

            // The classes only need to be set once (otherwise "all classes" will start to be added multiple times)
            if (Classes == null)
            {
                List<string> classes = new List<string>(await ItemRepository.GetClasses());

                // Add "all classes" to classes list
                classes.Add(SelectedClass);

                Classes = classes;
            }

            ReloadItemTypes(Items, true);

            List<string> itemSlots = await ItemRepository.GetItemSlots();
            // Add "all classes" to itemslots list
            itemSlots.Add(SelectedItemSlot);
            ItemSlots = itemSlots;
        }

        private void ReloadItemTypes(List<Item> items, bool addSelectedItemType = false)
        {
            List<string> itemTypes = new List<string>();

            foreach (Item item in items)
            {
                if (item.ItemType == null) continue;

                if (!itemTypes.Contains(item.ItemType))
                {
                    itemTypes.Add(item.ItemType);
                }
            }

            if (!addSelectedItemType)
            {
                if (!itemTypes.Contains(SelectedItemType)) SelectedItemType = "<all item types>";
            }

            // Add "all itemtypes" to itemtypes list
            itemTypes.Add(SelectedItemType);

            ItemTypes = itemTypes;
        }

        bool NeedsBigItemLibrary()
        {
            return !_selectedItemType.Equals("<all item types>") || (!_selectedItemSlot.Equals("<all item slots>") && !_selectedItemSlot.Equals("misc"));
        }

        private void GetItemsByClass(List<Item> items)
        {
            if (_selectedClass == null) return;

            if (!_selectedClass.Equals("<all classes>"))
            {
                for (int i = items.Count-1; i >= 0; --i)
                {
                    Item item = items[i];

                    if (item.Classes == null) continue;

                    if (!item.Classes.Contains(_selectedClass)) items.RemoveAt(i);
                }
            }
        }

        private void GetItemsByType(List<Item> items)
        {
            if (_selectedItemType == null) return;

            if (!_selectedItemType.Equals("<all item types>"))
            {
                for (int i = items.Count-1; i >= 0; --i)
                {
                    Item item = items[i];

                    if (item.ItemType == null) continue;

                    if (!item.ItemType.Equals(_selectedItemType)) items.RemoveAt(i);
                }
            }
        }

        private void GetItemsBySlot(List<Item> items)
        {
            if (_selectedItemSlot == null) return;

            if (!_selectedItemSlot.Equals("<all item slots>"))
            {
                for (int i = items.Count-1; i >= 0; --i)
                {
                    Item item = items[i];

                    if (item.ItemSlot == null) continue;

                    if (!item.ItemSlot.Equals(_selectedItemSlot)) items.RemoveAt(i);
                }
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
