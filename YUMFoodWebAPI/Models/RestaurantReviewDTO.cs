using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YUMFoodWebAPI.Models
{
    public class RestaurantReviewDTO
    {
        public string Review { get; set; }
        public int UserProfileId { get; set; }
        public int RestaurantId { get; set; }
    }
}