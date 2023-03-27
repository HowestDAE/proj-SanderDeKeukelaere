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
                _currentItem = value;
                ItemsInSet = ItemOverviewVM.ItemRepository.GetItemsInSet(_currentItem.ItemSet);
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
            Item item = new Item();
            item.ItemDescription = "Attack an enemy from behind to Backstab them for a one hit kill.";
            item.ItemName = "Knife";
            item.MaximumLevel = 1;
            item.MinimalLevel = 1;
            item.ItemQuality = 0;
            item.ImageUrl = "http://media.steampowered.com/apps/440/icons/w_knife_large.22ba9afe5d2266d3d9af0f7dcf99d67d588cb895.png";
            item.Classes = new List<string> { "Spy" };
            item.Capabilities = new Capabilities { CanBeGiftWrapped = true, CanBeKillstreak = true, CanBeStrangefied = true, Nameable = true, WillAttachCrafter = true };
            _currentItem = item;
        }
    }
}
