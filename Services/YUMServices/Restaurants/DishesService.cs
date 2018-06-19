using DataAccess.Model;
using Services.YUMServices.Base;
using Services.YUMServices.Exceptions;
using Services.YUMServices.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Infrastructure.Authorization.AuthorizationManager;

namespace Services.YUMServices.Restaurants
{
    public class DishesService : BaseAuthorizedService
    {
        private readonly Restaurant _restaurant;

        public DishesService(string userName) : base(userName, UserRole.RESTAURANT)
        {
            _restaurant = _unitOfWork.RestaurantRepository.Get(_id);
        }

        public IEnumerable<Dish> Get(int pageSize = 20, int pageNumber = 1)
        {
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            pageNumber -= 1;

            return _unitOfWork.DishRepository.Get(
                (d => d.RestaurantId == _id),
                (q => q.OrderByDescending(d => d.CreatedAt))
                ).Skip(pageNumber * pageSize).Take(pageSize);
        }

        public IEnumerable<Dish> GetByType(string dishType, int pageSize = 20, int pageNumber = 1)
        {
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            pageNumber -= 1;

            return _unitOfWork.DishRepository.Get(
             (d => (d.RestaurantId == _id) && (d.DishType.Name == dishType)),
             (q => q.OrderByDescending(d => d.CreatedAt))
             ).Skip(pageNumber * pageSize).Take(pageSize);
        }

        public int GetNumberOfPages(string filterOption = null, int pageSize = 20)
        {
            if (pageSize < 1)
            {
                pageSize = 20;
            }

            int numberOfDishes = _unitOfWork.DishRepository.Get(d => d.RestaurantId == _id).Count();

            return (int)Math.Ceiling((double)numberOfDishes / pageSize);
        }

        public Dish Get(int id)
        {
            return _unitOfWork.DishRepository.Get(
                d => (d.DishId == id) && (d.RestaurantId == _id)
                ).FirstOrDefault();
        }

        public void Remove(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(d => (d.DishId == dishId) && (d.RestaurantId == _id)).First();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            _unitOfWork.DishRepository.Remove(dish);
            _unitOfWork.Complete();
        }

        public void Add(Dish dish)
        {
            dish.CreatedAt = DateTime.Now;
            dish.RestaurantId = _id;

            _unitOfWork.DishRepository.Add(dish);
            _unitOfWork.Complete();
        }

        public IEnumerable<Allergen> GetAllergens(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(d => (d.DishId == dishId) && (d.RestaurantId == _id)).First();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            return dish.Allergens;
        }

        public void AddAllergen(int dishId, int allergenId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(d => (d.DishId == dishId) && (d.RestaurantId == _id)).First();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            Allergen allergen = _unitOfWork.AllergenRepository.Get(allergenId);

            if (allergen == null)
            {
                throw new EntityNotFoundException("Allergen with id " + allergenId + " not found.");
            }

            dish.Allergens.Add(allergen);

            _unitOfWork.DishRepository.Update(dish);
            _unitOfWork.Complete();
        }

        public void RemoveAllergen(int dishId, int allergenId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(d => (d.DishId == dishId) && (d.RestaurantId == _id)).First();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            Allergen allergen = _unitOfWork.AllergenRepository.Get(allergenId);

            if (allergen == null)
            {
                throw new EntityNotFoundException("Allergen with id " + allergenId + " not found.");
            }

            dish.Allergens.Remove(allergen);

            _unitOfWork.DishRepository.Update(dish);
            _unitOfWork.Complete();
        }

        public void Update(Dish dish)
        {
            Dish existingDish = _unitOfWork.DishRepository.Get(d => (d.DishId == dish.DishId) && (d.RestaurantId == _id)).First();

            if (existingDish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dish.DishId + " not found.");
            }

            dish.DishId = existingDish.DishId;

            _unitOfWork.DishRepository.Update(dish);
            _unitOfWork.Complete();
        }
    }
}
