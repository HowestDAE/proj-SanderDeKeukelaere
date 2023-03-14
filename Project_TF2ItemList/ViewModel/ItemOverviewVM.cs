using CommunityToolkit.Mvvm.ComponentModel;
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
        private List<Item> _items;
        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ItemOverviewVM()
        {
            _items = ItemRepository.GetItems();
        }
    }
}
