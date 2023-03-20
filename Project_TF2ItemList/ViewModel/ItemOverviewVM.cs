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
        IItemRepository _itemRepository = new APIItemRepository/*LocalItemRepository*/();

        private List<Item> _items;
        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public List<string> Classes
        {
            get { return classes; }
            set
            {
                classes = value;
                OnPropertyChanged(nameof(Classes));
            }
        }

        private string _selectedClass;
        private List<string> classes;

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


        public ItemOverviewVM()
        {
            LoadItemsAndClasses();
        }

        private async void LoadItemsAndClasses()
        {
            Items = await _itemRepository.GetItems();
            Classes = await _itemRepository.GetClasses();

            // Add "all classes" to classes list
            Classes.Add("<all classes>");
            SelectedClass = Classes[Classes.Count - 1];
        }

        private async void GetItems(string className)
        {
            if (_selectedClass.Equals("<all classes>"))
            {
                Items = await _itemRepository.GetItems();
            }
            else
            {
                Items = await _itemRepository.GetItems(_selectedClass);
            }
        }
    }
}
