using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace Project_TF2ItemList.Model
{
    public class Capabilities
    {
        [JsonProperty(PropertyName = "nameable")]
        public bool Nameable { get; set; }                  // Can an item be renamed

        [JsonProperty(PropertyName = "can_gift_wrap")]
        public bool CanBeGiftWrapped { get; set; }          // Can an item be gift wrapped

        [JsonProperty(PropertyName = "can_craft_mark")]
        public bool WillAttachCrafter { get; set; }         // Will the crafters name be attached after crafting this item

        [JsonProperty(PropertyName = "can_strangify")]
        public bool CanBeStrangefied { get; set; }          // Can this item be strangefied

        [JsonProperty(PropertyName = "can_killstreakify")]
        public bool CanBeKillstreak { get; set; }           // Can this item have a killstreak kit applied
    }
}
