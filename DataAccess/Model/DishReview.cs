//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class DishReview
    {
        public int DishReviewId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string Review { get; set; }
        public int UserProfileId { get; set; }
        public int DishId { get; set; }
    
        public virtual Dish Dish { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
