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

namespace YUMRestaurantWebAPI.Controllers
{
    [Authorize]
    public class RestaurantsController : ApiController
    {
        private RestaurantsService restaurantsService;

        // GET: api/Restaurants
        public IEnumerable<RestaurantDTO> GetRestaurants(
            string filterOption = null, string sortOption = null,
            int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                restaurantsService = new RestaurantsService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            IEnumerable<Restaurant> restaurants = restaurantsService.Get(
                filterOption, sortOption, pageSize, pageNumber);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurant, RestaurantDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            return iMapper.Map<IEnumerable<Restaurant>, IEnumerable<RestaurantDTO>>(restaurants);
        }

        // GET: api/Restaurants/5
        [ResponseType(typeof(RestaurantDTO))]
        public IHttpActionResult GetRestaurant(int id)
        {
            try
            {
                restaurantsService = new RestaurantsService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }

            Restaurant restaurant = restaurantsService.Get(id);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurant, RestaurantDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            RestaurantDTO restaurantDTO = iMapper.Map<Restaurant, RestaurantDTO>(restaurant);

            return Ok(restaurantDTO);
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