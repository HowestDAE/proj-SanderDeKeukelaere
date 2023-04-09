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
        int _pagesInApiCall = 5;
        const int ITEMS_PER_OVERVIEW_PAGE = 200;

        static public IItemRepository ItemRepository { get; set; }
        private APIItemRepository _apiRepository = new APIItemRepository();
        private LocalItemRepository _localRepository = new LocalItemRepository();

        private int _page = 0;

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

                // Refresh selected page and filtering
                RefreshOverview();

                // Update UI
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

                // Refresh selected page and filtering
                RefreshOverview(false);

                // Update UI
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

                // Refresh selected page and filtering
                RefreshOverview();

                // Update UI
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
        public RelayCommand LoadPageLeftCommand { get; private set; }
        public RelayCommand LoadPageRightCommand { get; private set; }

        public ItemOverviewVM()
        {
            ItemRepository = _apiRepository;
            LoadItemsAndClasses();

            SwitchRepositoryCommand = new RelayCommand(SwitchRepository);
            LoadPageLeftCommand = new RelayCommand(LoadPageLeft, HasPageLeft);
            LoadPageRightCommand = new RelayCommand(LoadPageRight, HasPageRight);
        }

        private void ResetPaging()
        {
            _page = 0;

            LoadPageLeftCommand.NotifyCanExecuteChanged();
            LoadPageRightCommand.NotifyCanExecuteChanged();
        }

        private void LoadPageLeft()
        {
            --_page;

            RefreshOverview(false, false);

            LoadPageLeftCommand.NotifyCanExecuteChanged();
            LoadPageRightCommand.NotifyCanExecuteChanged();
        }

        private void LoadPageRight()
        {
            ++_page;

            RefreshOverview(false, false);

            LoadPageLeftCommand.NotifyCanExecuteChanged();
            LoadPageRightCommand.NotifyCanExecuteChanged();
        }

        private bool HasPageLeft()
        {
            return _page > 0;
        }

        private bool HasPageRight()
        {
            return !ItemRepository.HasReachedEnd();
        }

        private async void RefreshOverview(bool reloadItemType = true, bool resetPaging = true)
        {
            // Reset paging to page 0
            if(resetPaging) ResetPaging();

            // Load the right page from the API
            int apiPage = NeedsBigItemLibrary() ? _page : _page / _pagesInApiCall;
            List<Item> itemsCopy = new List<Item>(await ItemRepository.GetItems(apiPage));

            // Get the nr of pages in the current api call
            _pagesInApiCall = itemsCopy.Count() / ITEMS_PER_OVERVIEW_PAGE;

            // If the filtering does not need a big library (filtering for itemtype or itemslot)
            if (!NeedsBigItemLibrary())
            {
                // Calculate the current page
                int overviewPage = _page - apiPage * _pagesInApiCall;

                // Remove items from the list at the end
                if (overviewPage < _pagesInApiCall - 1)
                {
                    int removeStartIdx = Math.Min(itemsCopy.Count() - 1, ITEMS_PER_OVERVIEW_PAGE * (overviewPage + 1));
                    itemsCopy.RemoveRange(removeStartIdx, itemsCopy.Count() - removeStartIdx);
                }

                // Remove items from the list at the front
                if (ITEMS_PER_OVERVIEW_PAGE * overviewPage > 0)
                    itemsCopy.RemoveRange(0, ITEMS_PER_OVERVIEW_PAGE * overviewPage);
            }

            // Filter by class and itemslot
            GetItemsByClass(itemsCopy);
            GetItemsBySlot(itemsCopy);

            // Update the ItemType filter if needed
            if(reloadItemType) ReloadItemTypes(itemsCopy);

            // Filter by itemtype
            GetItemsByType(itemsCopy);

            // Set the items container
            Items = itemsCopy;
        }

        private async void LoadItemsAndClasses()
        {
            List<Item> itemsCopy = new List<Item>(await ItemRepository.GetItems(0));
            itemsCopy.RemoveRange(ITEMS_PER_OVERVIEW_PAGE, itemsCopy.Count() - ITEMS_PER_OVERVIEW_PAGE);
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

                    if (item.ItemSlot == null || !item.Classes.Contains(_selectedClass)) items.RemoveAt(i);
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

                    if (item.ItemSlot == null || !item.ItemType.Equals(_selectedItemType)) items.RemoveAt(i);
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

                    if (item.ItemSlot == null || !item.ItemSlot.Equals(_selectedItemSlot)) items.RemoveAt(i);
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
