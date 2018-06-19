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
    public class AllergensController : ApiController
    {
        private YUMFoodEntities db = new YUMFoodEntities();

        // GET: api/Allergens
        public IQueryable<Allergen> GetAllergens()
        {
            return db.Allergens;
        }

        // GET: api/Allergens/5
        [ResponseType(typeof(Allergen))]
        public IHttpActionResult GetAllergen(int id)
        {
            Allergen allergen = db.Allergens.Find(id);
            if (allergen == null)
            {
                return NotFound();
            }

            return Ok(allergen);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AllergenExists(int id)
        {
            return db.Allergens.Count(e => e.AllergenId == id) > 0;
        }
    }
}