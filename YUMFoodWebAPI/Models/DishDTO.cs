using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YUMFoodWebAPI.Models
{
    public class DishDTO
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public Nullable<float> Discount { get; set; }
        public int PreparationTime { get; set; }
        public string ImageURL { get; set; }
        public int RestaurantId { get; set; }
        public int DishTypeId { get; set; }
    }
}