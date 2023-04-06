using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    public class APIItemRepository : IItemRepository
    {
        private List<Item> _items;
        private List<string> _classes = new List<string>();
        private List<string> _itemTypes = new List<string>();
        private List<string> _itemSlots = new List<string>();

        private async Task LoadItemsAndClassesAsync()
        {
            string endpoint = "https://api.steampowered.com/IEconItems_440/GetSchemaItems/v1/?key=A905BA577D8E3D98AEA5C5F5CDF05AE9&language=en";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // send q POST to the API + catch the result
                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode) // OK?
                        throw new HttpRequestException(response.ReasonPhrase);

                    // read the result json string asynchronously
                    string json = await response.Content.ReadAsStringAsync();

                    _items = JsonConvert.DeserializeObject<JObject>(json).SelectToken("result").SelectToken("items").ToObject<List<Item>>();
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to load items using the API");
                }
            }

            // If the item list doesn't exist, make an empty item list
            if (_items == null) _items = new List<Item>();

            // Remove items without image
            _items.RemoveAll(item => item.ImageUrl == null || item.ImageUrl.Length == 0);

            await Task.Run(() =>
            {
                // Remove doubles
                for(int i = 0; i < _items.Count; ++i)
                {
                    Item curItem = _items[i];

                    if (curItem.Classes == null) continue;

                    // Get all items with the same name as the current item
                    List<Item> doubleItems = _items.FindAll(otherItem => otherItem != curItem && otherItem.ItemName.Equals(curItem.ItemName) && otherItem.ItemSlot.Equals(curItem.ItemSlot));

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
            });

            _items.RemoveRange(200, _items.Count() - 200);

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

                if (_classes.Count() == 9) break;
            }
        }

        public async Task<List<string>> GetClasses()
        {
            if (_classes == null) await LoadItemsAndClassesAsync();

            return _classes;
        }

        public async Task<List<Item>> GetItems()
        {
            if (_items == null) await LoadItemsAndClassesAsync();

            return _items;
        }

        public List<Item> GetItemsInSet(string itemSet)
        {
            List<Item> items = new List<Item>();

            if (itemSet == null) return items;

            foreach (Item item in _items)
            {
                if (item.ItemSet == null) continue;

                if (item.ItemSet.Equals(itemSet)) items.Add(item);
            }

            return items;
        }

        public async Task<List<string>> GetItemTypes()
        {
            if(_items == null) await LoadItemsAndClassesAsync();

            _itemTypes = new List<string>();

            foreach(Item item in _items)
            {
                if(item.ItemType == null) continue;

                if(!_itemTypes.Contains(item.ItemType))
                {
                    _itemTypes.Add(item.ItemType);
                }
            }

            return _itemTypes;
        }

        public async Task<List<string>> GetItemSlots()
        {
            if (_items == null) await LoadItemsAndClassesAsync();

            _itemSlots = new List<string>();

            foreach (Item item in _items)
            {
                if (item.ItemSlot == null) continue;

                if (!_itemSlots.Contains(item.ItemSlot))
                {
                    _itemSlots.Add(item.ItemSlot);
                }
            }

            return _itemSlots;
        }
    }
}
