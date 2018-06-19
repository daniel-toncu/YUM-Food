using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess.Model;

namespace YUMRestaurantWebAPI.Controllers
{
    public class DishTypesController : ApiController
    {
        private YUMFoodEntities db = new YUMFoodEntities();

        // GET: api/DishTypes
        public IQueryable<DishType> GetDishTypes()
        {
            return db.DishTypes;
        }

        // GET: api/DishTypes/5
        [ResponseType(typeof(DishType))]
        public IHttpActionResult GetDishType(int id)
        {
            DishType dishType = db.DishTypes.Find(id);
            if (dishType == null)
            {
                return NotFound();
            }

            return Ok(dishType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DishTypeExists(int id)
        {
            return db.DishTypes.Count(e => e.DishTypeId == id) > 0;
        }
    }
}