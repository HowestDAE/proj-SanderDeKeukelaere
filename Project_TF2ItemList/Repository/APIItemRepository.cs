using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_TF2ItemList.Repository
{
    public class APIItemRepository : IItemRepository
    {
        private List<Item> _items;
        private List<string> _classes = new List<string>();
        private List<string> _itemSlots = new List<string>();

        private int _curPage = -1;

        private List<int> _prevItemIndices = new List<int>();
        private int _nextItemIdx = 1000;
        private int _curItemIdx = 0;

        private async Task LoadItemsAndClassesAsync()
        {
            string endpoint = $"https://api.steampowered.com/IEconItems_440/GetSchemaItems/v1/?key=A905BA577D8E3D98AEA5C5F5CDF05AE9&language=en&start={_curItemIdx}";

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

                    JToken result = JsonConvert.DeserializeObject<JObject>(json).SelectToken("result");

                    _items = result.SelectToken("items").ToObject<List<Item>>();

                    JToken nextIdx = result.SelectToken("next");
                    if(nextIdx != null)
                    {
                        _nextItemIdx = nextIdx.ToObject<int>();
                    }
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
            });

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

            // Items with no classes assigned, should have all classes
            foreach (Item item in _items)
            {
                if (item.Classes != null && item.Classes.Count() > 0) continue;
                if (item.ItemSlot == null) continue; // Item should be equipable

                item.Classes = _classes;
            }
        }

        public async Task<List<string>> GetClasses()
        {
            if (_classes == null) await LoadItemsAndClassesAsync();

            return _classes;
        }

        public async Task<List<Item>> GetItems(int page)
        {
            if (_curPage == page) return _items;

            if (_curPage == -1)
            {
                // This is the first time this function is called, so set the itemidx to 0
                _curItemIdx = 0;
            }
            else
            {
                // Add the current itemidx to the list of previous indices if needed
                if (page > 0 && _prevItemIndices.Count < page) _prevItemIndices.Add(_curItemIdx);

                // Set the current itemidx to the previous or next page
                _curItemIdx = page > _curPage ? _nextItemIdx : _prevItemIndices[page];

                // Make sure that next item idx is not the same as the current item idx
                ++_nextItemIdx;
            }

            // Set the page number
            _curPage = page;

            // Reload items
            await LoadItemsAndClassesAsync();

            return _items;
        }

        public async Task<List<Item>> GetItemsInSet(string itemSet)
        {
            if (_items == null) await LoadItemsAndClassesAsync();

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
            if (_items == null) await LoadItemsAndClassesAsync();

            _itemSlots = new List<string>();

            // Fill the list with all the item slots
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

        public bool HasReachedEnd()
        {
            // If the API has reached the end, it will return the same next item idx as the current item idx
            return _curItemIdx == _nextItemIdx;
        }
    }
}
