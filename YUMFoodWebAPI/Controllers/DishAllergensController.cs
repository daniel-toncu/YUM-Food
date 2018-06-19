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
using AutoMapper;
using DataAccess.Infrastructure.Authorization.Exceptions;
using DataAccess.Model;
using Services.YUMServices.Clients;
using YUMFoodWebAPI.Models;

namespace YUMFoodWebAPI.Controllers
{
    [Authorize]
    public class DishAllergensController : ApiController
    {
        private DishesService dishesService;

        // GET: api/Allergens
        public IEnumerable<AllergenDTO> GetAllergens(int id)
        {
            try
            {
                dishesService = new DishesService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            IEnumerable<Allergen> allergens = dishesService.GetAllergens(id);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Allergen, AllergenDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            return iMapper.Map<IEnumerable<Allergen>, IEnumerable<AllergenDTO>>(allergens);
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