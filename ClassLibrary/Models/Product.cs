using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ModelLayer.Models;
using DataLayer.Base;


namespace ClassLibrary
{
    public class Product : BaseEntity
    {

        [Display(Name = "Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Display(Name = "Brand")]
        [MaxLength(20)]
        public string? Brand { get; set; }

        [Display(Name = "Code")]
        [MaxLength(20)]
        public string? ProductCode { get; set; }

        [Display(Name = "نوع محصول")]
        public ProductType Type { get; set; }

        [Display(Name="Image file")]
        public string? ImagePath { get; set; }

        [Precision(16 , 2)]
        [Display(Name = "قیمت")]
        public decimal Price {  get; set; }

        public ICollection<Order> Sales { get; set; } = new List<Order>();

        public ICollection<Customer>? customers { get; set; } // Navigation property

        public ICollection<Shop>? sellers { get; set; }
    }
}

public enum ProductType
{
    [Display(Name = "T-Shirt")]
    TShirt = 0,
    [Display(Name = "Bracelet")]
    Bracelet = 1,
    [Display(Name = "Lighter")]
    Lighter = 2,
    [Display(Name = "Thermos")]
    Thermos = 3,
    [Display(Name = "Solar Charger")]
    SolarCharger = 4,
}

