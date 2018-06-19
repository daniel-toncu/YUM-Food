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
    public class RestaurantReviewsController : ApiController
    {
        private RestaurantsService restaurantsService;

        // GET: api/RestaurantReviews/5
        public IEnumerable<RestaurantReviewDTO> GetRestaurantReview(int id)
        {
            try
            {
                restaurantsService = new RestaurantsService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            IEnumerable<RestaurantReview> restaurantReviews = restaurantsService.GetReviews(id);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestaurantReview, RestaurantReviewDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            return iMapper.Map<IEnumerable<RestaurantReview>, IEnumerable<RestaurantReviewDTO>>(restaurantReviews);
        }

        // POST: api/RestaurantReviews
        [ResponseType(typeof(RestaurantReviewDTO))]
        public IHttpActionResult PostRestaurantReview(RestaurantReviewDTO restaurantReviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                restaurantsService = new RestaurantsService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestaurantReviewDTO, RestaurantReview>();
            });
            IMapper iMapper = config.CreateMapper();

            RestaurantReview restaurantReview = iMapper.Map<RestaurantReviewDTO, RestaurantReview>(restaurantReviewDTO);

            restaurantsService.AddReview(restaurantReview);

            return CreatedAtRoute("DefaultApi", new { id = restaurantReview.RestaurantReviewId }, restaurantReviewDTO);
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