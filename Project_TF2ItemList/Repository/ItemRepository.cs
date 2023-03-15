﻿using Newtonsoft.Json;
using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    public class ItemRepository
    {
        private static List<Item> _items;
        private static List<string> _classes = new List<string>();

        public static List<Item> GetItems()
        {
            // If item list already exists, return it
            if (_items != null) return _items;

            // Get assembly
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            // Embedded file
            const string resourceName = "Project_TF2ItemList.Resources.Data.items.json";

            // Load resource from assembly
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                // Open a stream reader
                using (var reader = new StreamReader(stream))
                {
                    // Read all text in the resource
                    string json = reader.ReadToEnd();

                    // Convert the json to a list of pokemons
                    _items = JsonConvert.DeserializeObject<ItemSchema>(json).Items.ToList();
                }
            }

            // If the item list doesn't exist, make an empty item list
            if (_items == null) _items = new List<Item>();

            // Remove doubles
            for(int i = 0; i < _items.Count(); ++i)
            {
                Item curItem = _items[i];

                // Get all items with the same name as the current item
                List<Item> doubleItems = _items.FindAll(otherItem => otherItem != curItem && otherItem.ItemName.Equals(curItem.ItemName));

                foreach (Item doubleItem in doubleItems)
                {
                    // Combine the classes list
                    curItem.Classes.AddRange(doubleItem.Classes);

                    // Remove the double from the itemlist
                    _items.Remove(doubleItem);
                }
            }

            // Get all classes
            foreach(Item item in _items)
            {
                if (item.Classes.Count == 0) continue;

                foreach(string className in item.Classes)
                {
                    if(_classes.Contains(className)) continue;

                    _classes.Add(className);
                }
            }

            // Return the item list
            return _items;
        }

        public static List<Item> GetItems(string className)
        {
            GetItems();

            List<Item> items = new List<Item>();

            foreach(Item item in _items)
            {
                if (item.Classes.Contains(className)) items.Add(item);
            }

            return items;
        }

        public static List<string> GetClasses()
        {
            GetItems();

            return _classes;
        }
    }
}
