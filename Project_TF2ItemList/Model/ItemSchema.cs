using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Model
{
    public class ItemSchema
    {
        [JsonProperty(PropertyName = "items")]
        public Item[] Items { get; set; }
    }
}
