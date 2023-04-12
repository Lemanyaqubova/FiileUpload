﻿namespace FirstApii.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public double SalePrice { get; set; }
        public double CostPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? CategoryId{ get; set; }
        public Category Category { get; set; }
    }
}
