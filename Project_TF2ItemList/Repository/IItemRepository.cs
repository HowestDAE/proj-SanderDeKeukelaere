using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    internal interface IItemRepository
    {
        List<Item> GetItems();
        List<Item> GetItems(string className);
        List<string> GetClasses();
    }
}
