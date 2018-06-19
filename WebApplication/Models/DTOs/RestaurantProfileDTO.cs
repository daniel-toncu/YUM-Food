using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.DTOs
{
    public class RestaurantProfileDTO
    {
        [ReadOnly(true)]
        public int RestaurantId { get; set; }

        [ReadOnly(true)]
        public System.DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [DataType(DataType.Time)]
        public Nullable<System.TimeSpan> OpenAt { get; set; }

        [DataType(DataType.Time)]
        public Nullable<System.TimeSpan> CloseAt { get; set; }

        [Range(0, 100)]
        public Nullable<float> MinOrderFreeDelivery { get; set; }

        [Range(0, 100)]
        public Nullable<float> DeliveryPrice { get; set; }

        [DataType(DataType.Time)]
        public System.TimeSpan DeliveryTime { get; set; }

        public string LogoURL { get; set; }

        public string CoverURL { get; set; }
    }
}