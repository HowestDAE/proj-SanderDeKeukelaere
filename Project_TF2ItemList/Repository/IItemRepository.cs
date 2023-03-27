﻿using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    internal interface IItemRepository
    {
        Task<List<Item>> GetItems();
        Task<List<Item>> GetItems(string className);
        Task<List<string>> GetClasses();
        Task<List<Item>> GetItemsInSet(string itemSet);
    }
}
