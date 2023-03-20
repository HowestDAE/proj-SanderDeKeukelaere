using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    public class APIItemRepository : IItemRepository
    {
        private List<Item> _items;
        private List<string> _classes = new List<string>();

        public List<string> GetClasses()
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItems()
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItems(string className)
        {
            throw new NotImplementedException();
        }
    }
}
