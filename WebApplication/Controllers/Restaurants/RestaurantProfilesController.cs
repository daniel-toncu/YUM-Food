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
using DataAccess.Model;
using Services.YUMServices.Common;
using Services.YUMServices.Restaurants;
using WebApplication.Models.DTOs;

namespace WebApplication.Controllers.Restaurants
{
    [Authorize]
    public class RestaurantProfilesController : ApiController
    {
        private ProfilesService profilesService;
        private UsersService usersService;


        // GET: yum-api/RestaurantProfile
        [ResponseType(typeof(RestaurantProfileDTO))]
        public IHttpActionResult GetRestaurant()
        {
            profilesService = new ProfilesService(User.Identity.Name);

            Restaurant restaurant = profilesService.Get();

            if (restaurant == null)
            {
                return NotFound();
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurant, RestaurantProfileDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            return Ok(iMapper.Map<Restaurant, RestaurantProfileDTO>(restaurant));
        }

        // PUT: yum-api/RestaurantProfile
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurant(RestaurantProfileDTO restaurantProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            profilesService = new ProfilesService(User.Identity.Name);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestaurantProfileDTO, Restaurant>();
            });
            IMapper iMapper = config.CreateMapper();

            Restaurant restaurant = iMapper.Map<RestaurantProfileDTO, Restaurant>(restaurantProfileDTO);

            profilesService.Update(restaurant);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: yum-api/RestaurantProfiles
        public IHttpActionResult PostRestaurant(RestaurantProfileDTO restaurantProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestaurantProfileDTO, Restaurant>();
            });
            IMapper iMapper = config.CreateMapper();

            Restaurant restaurant = iMapper.Map<RestaurantProfileDTO, Restaurant>(restaurantProfileDTO);

            usersService = new UsersService();

            usersService.RegisterRestaurant(User.Identity.Name, restaurant);

            return Ok();
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