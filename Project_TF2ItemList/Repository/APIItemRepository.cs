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
            _items.RemoveAll(item => item.ImageUrl == null);

            await Task.Run(() =>
            {
                // Remove doubles
                for(int i = 0; i < _items.Count; ++i)
                {
                    Item curItem = _items[i];

                    if (curItem.Classes == null) continue;

                    // Get all items with the same name as the current item
                    List<Item> doubleItems = _items.FindAll(otherItem => otherItem != curItem && otherItem.ItemName.Equals(curItem.ItemName));

                    foreach (Item doubleItem in doubleItems)
                    {
                        if (doubleItem.Classes == null) continue;

                        // Combine the classes list
                        foreach (string className in doubleItem.Classes)
                        {
                            if (curItem.Classes.Contains(className)) continue;

                            curItem.Classes.Add(className);
                        }

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

        public async Task<List<Item>> GetItems(string className)
        {
            if (_items == null) await LoadItemsAndClassesAsync();

            List<Item> items = new List<Item>();

            foreach (Item item in _items)
            {
                if (item.Classes == null) continue;

                if (item.Classes.Contains(className)) items.Add(item);
            }

            return items;
        }

        public async Task<List<Item>> GetItemsInSet(string itemSet)
        {
            await GetItems();

            List<Item> items = new List<Item>();

            foreach (Item item in _items)
            {
                if (item.ItemSet.Equals(itemSet)) items.Add(item);
            }

            return items;
        }
    }
}
