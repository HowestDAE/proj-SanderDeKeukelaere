using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    public class LocalItemRepository : IItemRepository
    {
        private List<Item> _items;
        private List<string> _classes = new List<string>();
        private List<string> _itemSlots = new List<string>();

        public async Task<List<Item>> GetItems(int page = 0)
        {
            // If item list already exists, return it
            if (_items != null) return _items;

            // Get assembly
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            // Embedded file
            const string resourceName = "Project_TF2ItemList.Resources.Data.items.json";

            await Task.Run(() =>
            {
                // Load resource from assembly
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    // Open a stream reader
                    using (var reader = new StreamReader(stream))
                    {
                        // Read all text in the resource
                        string json = reader.ReadToEnd();

                        // Convert the json to a list of items
                        JToken result = JsonConvert.DeserializeObject<JObject>(json).SelectToken("result");

                        _items = result.SelectToken("items").ToObject<List<Item>>();
                    }
                }

                // If the item list doesn't exist, make an empty item list
                if (_items == null) _items = new List<Item>();

                // Remove doubles
                for (int i = 0; i < _items.Count; ++i)
                {
                    Item curItem = _items[i];

                    if (curItem.Classes == null) continue;

                    // Get all items with the same name as the current item
                    List<Item> doubleItems = _items.FindAll(otherItem => otherItem != curItem && otherItem.ItemName.Equals(curItem.ItemName) && (otherItem.ItemSlot == null || curItem.ItemSlot == null || otherItem.ItemSlot.Equals(curItem.ItemSlot)));

                    foreach (Item doubleItem in doubleItems)
                    {
                        if (doubleItem.Classes == null) continue;

                        // Combine the classes list
                        // Items with more then 1 class are doubles and need to be ignored
                        if (doubleItem.Classes.Count == 1 && !curItem.Classes.Contains(doubleItem.Classes[0]))
                            curItem.Classes.Add(doubleItem.Classes[0]);

                        // Remove the double from the itemlist
                        _items.Remove(doubleItem);
                    }
                }

                // Get all classes
                foreach (Item item in _items)
                {
                    if (item.Classes == null) continue;

                    if (item.Classes.Count == 0) continue;

                    foreach (string className in item.Classes)
                    {
                        if (_classes.Contains(className)) continue;

                        _classes.Add(className);
                    }
                }
            });

            // Return the item list
            return _items;
        }

        public async Task<List<string>> GetClasses()
        {
            await GetItems();

            return _classes;
        }

        public async Task<List<Item>> GetItemsInSet(string itemSet)
        {
            await GetItems();

            List<Item> items = new List<Item>();

            // If no item set is given, return an empty list
            if (itemSet == null) return items;

            // Fill the list with all the items that are in the given item set
            foreach (Item item in _items)
            {
                if (item.ItemSet == null) continue;

                if (item.ItemSet.Equals(itemSet)) items.Add(item);
            }

            return items;
        }

        public async Task<List<string>> GetItemSlots()
        {
            await GetItems();

            _itemSlots = new List<string>();

            // Fill the list with all the item slots
            foreach (Item item in _items)
            {
                if (item.ItemSlot == null) continue;

                if (!_itemSlots.Contains(item.ItemSlot)) _itemSlots.Add(item.ItemSlot);
            }

            return _itemSlots;
        }

        public bool HasReachedEnd()
        {
            // The local repository only has one page, so it has always reached the end of its data
            return true;
        }
    }
}
