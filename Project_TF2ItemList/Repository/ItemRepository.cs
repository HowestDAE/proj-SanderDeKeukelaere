using Newtonsoft.Json;
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
        private static Item[] _items;

        public static Item[] GetItems()
        {
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
                    _items = JsonConvert.DeserializeObject<ItemSchema>(json).Items;
                }
            }

            if (_items == null) _items = new Item[] { };

            return _items;
        }
    }
}
