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
        public bool Nameable { get; set; }

        [JsonProperty(PropertyName = "can_gift_wrap")]
        public bool CanBeGiftWrapped { get; set; }

        [JsonProperty(PropertyName = "can_craft_mark")]
        public bool WillAttachCrafter { get; set; }

        [JsonProperty(PropertyName = "can_strangify")]
        public bool CanBeStrangefied { get; set; }

        [JsonProperty(PropertyName = "can_killstreakify")]
        public bool CanBeKillstreak { get; set; }
    }
}
