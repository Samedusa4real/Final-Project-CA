﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend___Putka.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal SalePrice { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Range(0, 100)]
        public int DiscountPercent { get; set; }
        [Required]
        public bool StockStatus { get; set; }
        [Required]
        public bool IsNew { get; set; }
        public DateTime ProductionDate { get; set; } 
        public Category Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
