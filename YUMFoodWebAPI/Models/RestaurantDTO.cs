using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YUMFoodWebAPI.Models
{
    public class RestaurantDTO
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public Nullable<System.TimeSpan> OpenAt { get; set; }
        public Nullable<System.TimeSpan> CloseAt { get; set; }
        public Nullable<float> MinOrderFreeDelivery { get; set; }
        public Nullable<float> DeliveryPrice { get; set; }
        public System.TimeSpan DeliveryTime { get; set; }
        public string LogoURL { get; set; }
        public string CoverURL { get; set; }
    }
}