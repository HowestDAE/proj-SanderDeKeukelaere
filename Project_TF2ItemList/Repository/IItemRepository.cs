﻿using Project_TF2ItemList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_TF2ItemList.Repository
{
    public interface IItemRepository
    {
        Task<List<Item>> GetItems(int page);
        Task<List<string>> GetClasses();
        Task<List<string>> GetItemSlots();
        Task<List<Item>> GetItemsInSet(string itemSet);
        bool HasReachedEnd();
    }
}
