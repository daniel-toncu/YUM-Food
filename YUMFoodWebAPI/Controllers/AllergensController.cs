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
using DataAccess.Infrastructure.Authorization.Exceptions;
using DataAccess.Model;
using Services.YUMServices.Clients;

namespace YUMFoodWebAPI.Controllers
{
    [Authorize]
    public class AllergensController : ApiController
    {
        private ProfilesService profilesService;

        // GET: api/Allergens
        public IEnumerable<Allergen> GetAllergens()
        {
            try
            {
                profilesService = new ProfilesService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            return profilesService.GetAllergens();
        }

        // PUT: api/Allergens/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAllergen(int id)
        {
            try
            {
                profilesService = new ProfilesService(User.Identity.Name);
                profilesService.AddAllergen(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Allergens/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteAllergen(int id)
        {
            try
            {
                profilesService = new ProfilesService(User.Identity.Name);
                profilesService.RemoveAllergen(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}