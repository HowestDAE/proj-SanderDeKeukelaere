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
            set
            { 
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public List<string> Classes { get; set; }

        private string _selectedClass;

        public string SelectedClass
        {
            get { return _selectedClass; }
            set 
            { 
                _selectedClass = value;
                if (_selectedClass.Equals("<all classes>"))
                {
                    Items = ItemRepository.GetItems();
                }
                else
                {
                    Items = ItemRepository.GetItems(_selectedClass);
                }
                OnPropertyChanged(nameof(SelectedClass));
            }
        }


        public ItemOverviewVM()
        {
            Items = ItemRepository.GetItems();
            Classes = ItemRepository.GetClasses();

            // Add "all classes" to classes list
            Classes.Add("<all classes>");
            SelectedClass = Classes[Classes.Count - 1];
        }
    }
}
