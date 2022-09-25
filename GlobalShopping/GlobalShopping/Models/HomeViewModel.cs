﻿using GlobalShopping.Common;
using GlobalShopping.Data.Entities;

namespace GlobalShopping.Models
{
    public class HomeViewModel
    {

        public PaginatedList<Product> Products { get; set; }

        public ICollection<Category> Categories { get; set; }
        public float Quantity { get; set; }

    }
}
