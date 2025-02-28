﻿namespace NewEraAPI.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Foreign key for Category
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; } // Virtual for lazy loading
    }
}
