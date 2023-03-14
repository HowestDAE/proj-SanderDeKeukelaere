using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project_TF2ItemList.Model
{
    public class Item
    {
        [JsonProperty(PropertyName = "item_name")]
        public string ItemName { get; set; }                // The actual name of the weapon

        [JsonProperty(PropertyName = "item_slot")]
        public string ItemSlot { get; set; }                // Which slot does this item go into

        [JsonProperty(PropertyName = "item_quality")]
        public int ItemQuality { get; set; }                // Which quality does this item have

        [JsonProperty(PropertyName = "min_ilevel")]
        public int MinimalLevel { get; set; }               // The minimal possible level of the item

        [JsonProperty(PropertyName = "max_ilevel")]
        public int MaximumLevel { get; set; }               // The maximum possible level of the item

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }                // Url to the image of the item

        [JsonProperty(PropertyName = "craft_class")]
        public string ItemType { get; set; }                // What type of item is it

        [JsonProperty(PropertyName = "capabilities")]
        public Capabilities Capabilities { get; set; }      // What can the user do with this item

        [JsonProperty(PropertyName = "used_by_classes")]
        public string[] Classes { get; set; }               // Which classes use this item

        [JsonProperty(PropertyName = "item_description")]
        public string ItemDescription { get; set; }         // The description of this item

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ItemName);
            if(ItemDescription != null && ItemDescription.Length > 0 )
            {
                sb.Append($" - {ItemDescription}");
            }

            sb.Append(" - Used by: ");

            foreach(string user in Classes)
            {
                sb.Append($"{user} ");
            }

            return sb.ToString();
        }
    }
}
