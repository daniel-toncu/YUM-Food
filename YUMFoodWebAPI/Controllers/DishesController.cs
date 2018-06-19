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
    public class DishesController : ApiController
    {
        private DishesService dishesService;

        // GET: api/Dishes
        public IEnumerable<DishDTO> GetDishes(
            string filterOption = null, string sortOption = null,
            int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                dishesService = new DishesService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return null;
            }

            IEnumerable<Dish> dishes = dishesService.Get(
                filterOption, sortOption, pageSize, pageNumber);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dish, DishDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            return iMapper.Map<IEnumerable<Dish>, IEnumerable<DishDTO>>(dishes);
        }

        // GET: api/Dishes/5
        [ResponseType(typeof(DishDTO))]
        public IHttpActionResult GetDish(int id)
        {
            try
            {
                dishesService = new DishesService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }

            Dish dish = dishesService.Get(id);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dish, DishDTO>();
            });
            IMapper iMapper = config.CreateMapper();

            DishDTO dishDTO = iMapper.Map<Dish, DishDTO>(dish);

            return Ok(dishDTO);
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