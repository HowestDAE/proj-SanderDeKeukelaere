using CommunityToolkit.Mvvm.ComponentModel;
using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.ViewModel
{
    public class ItemInspectionVM : ObservableObject
    {
        private Item _currentItem;

        public Item CurrentItem
        {
            get { return _currentItem; }
            set
            {
                // Do nothing if the item is set to the same value
                if (_currentItem == value) return;

                _currentItem = value;

                // Get the current item set
                GetItemSet();

                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        private List<Item> _itemsInSet = new List<Item>();
        public List<Item> ItemsInSet
        {
            get { return _itemsInSet; }
            set
            {
                _itemsInSet = value;
                OnPropertyChanged(nameof(ItemsInSet));
            }
        }

        public ItemInspectionVM()
        {
            // Create a default item to display
            Item item = new Item
            {
                ItemDescription = "Attack an enemy from behind to Backstab them for a one hit kill.",
                ItemName = "Knife",
                MaximumLevel = 1,
                MinimalLevel = 1,
                ItemQuality = 0,
                ImageUrl = "http://media.steampowered.com/apps/440/icons/w_knife_large.22ba9afe5d2266d3d9af0f7dcf99d67d588cb895.png",
                Classes = new List<string> { "Spy" },
                Capabilities = new Capabilities { CanBeGiftWrapped = true, CanBeKillstreak = true, CanBeStrangefied = true, Nameable = true, WillAttachCrafter = true }
            };
            _currentItem = item;
        }

        private async void GetItemSet()
        {
            if (_currentItem != null) 
                ItemsInSet = await ItemOverviewVM.ItemRepository.GetItemsInSet(_currentItem.ItemSet);
            else 
                ItemsInSet = new List<Item>();
        }
    }
}
