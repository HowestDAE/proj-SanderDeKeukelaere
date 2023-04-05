using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Project_TF2ItemList.Model
{
    public class Purpose
    {
        private string _purposeName;

        [JsonProperty(PropertyName = "type")]
        public string PurposeName
        {
            get 
            {
                if (_purposeName != null) _purposeName = _purposeName.Replace('_', ' ');
                return _purposeName;
            }
            set { _purposeName = value; }
        }
    }

    public class Item
    {
        private string _itemName;
        [JsonProperty(PropertyName = "item_name")]
        public string ItemName                              // The actual name of the weapon
        {
            get 
            {
                if (HasProperName)
                {
                    if (!_itemName.StartsWith("The"))
                    {
                        _itemName = $"The {_itemName}";
                    }
                }
                return _itemName; 
            }
            set { _itemName = value; }
        }

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

        [JsonProperty(PropertyName = "item_type_name")]
        public string ItemType { get; set; }                // What type of item is it

        private string _itemSet;
        [JsonProperty(PropertyName = "item_set")]
        public string ItemSet                               // What set is this item part of
        {
            get
            {
                if (_itemSet == null) return null;
                return _itemSet.Replace('_', ' ');
            }

            set { _itemSet = value; }
        }

        [JsonProperty(PropertyName = "capabilities")]
        public Capabilities Capabilities { get; set; }      // What can the user do with this item

        [JsonProperty(PropertyName = "used_by_classes")]
        public List<string> Classes { get; set; }           // Which classes use this item

        [JsonProperty(PropertyName = "item_description")]
        public string ItemDescription { get; set; }         // The description of this item

        [JsonProperty(PropertyName = "craft_class")]
        public string CraftClass { get; set; }              // How can this item be used in crafting

        private string _holidayRestriction;
        [JsonProperty(PropertyName = "holiday_restriction")]
        public string HolidayRestriction                    // Does this item have an holiday restriction
        {
            get
            {
                if (_holidayRestriction != null) _holidayRestriction = _holidayRestriction.Replace('_', ' ');
                return _holidayRestriction; 
            }
            set { _holidayRestriction = value; }
        }

        [JsonProperty(PropertyName = "tool")]
        public Purpose ItemPurpose { get; set; }            // What can this item be used for

        [JsonProperty(PropertyName = "proper_name")]
        public bool HasProperName { get; set; }             // Should the display name of this item start with "the"

        [JsonIgnore]
        public string Level
        {
            get { return MinimalLevel == MaximumLevel ? MinimalLevel.ToString() : $"{MinimalLevel}-{MaximumLevel}"; }
        }

        [JsonIgnore]
        public string SubTitle
        {
            get { return $"Level {Level} {ItemType}"; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ItemName);
            if (ItemDescription != null && ItemDescription.Length > 0)
            {
                sb.Append($" - {ItemDescription}");
            }

            if (Classes != null)
            {
                sb.Append(" - Used by: ");

                foreach (string user in Classes)
                {
                    sb.Append($"{user} ");
                }
            }

            return sb.ToString();
        }
    }
}
