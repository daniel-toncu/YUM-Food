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

namespace Services.YUMServices.Clients
{
    public class DishesService : BaseAuthorizedService
    {
        private readonly UserProfile _userProfile;

        private FilterOptions<Dish> filterOptions;
        private SortOptions<Dish> sortOptions;

        private FilterOptions<Dish> FilterOptions
        {
            get
            {
                if (filterOptions == null)
                {
                    filterOptions = new FilterOptions<Dish>(d => (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City));

                    filterOptions.AddOption("open_now", "Restaurante Deschise Acum",
                        (d => ((d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City))
                        && ((d.Restaurant.OpenAt == null) || (d.Restaurant.CloseAt == null) || ((d.Restaurant.OpenAt < DateTime.Now.TimeOfDay) && (d.Restaurant.CloseAt > DateTime.Now.TimeOfDay)))));

                    filterOptions.AddOption("free_delivery", "Restaurante Livrări Gratis",
                        (d => ((d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)) && (d.Restaurant.MinOrderFreeDelivery != null)));

                    filterOptions.AddOption("ignore_allergens", "Ignoră Alergeni",
                      (d => ((d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)) && (d.Allergens.Any(a => _userProfile.Allergens.Contains(a)))));
                }

                return filterOptions;
            }
        }

        private SortOptions<Dish> SortOptions
        {
            get
            {
                if (sortOptions == null)
                {
                    sortOptions = new SortOptions<Dish>(q => q.OrderBy(d => d.Name));

                    sortOptions.AddOption("name_asc", "Nume", (q => q.OrderBy(d => d.Name)));

                    sortOptions.AddOption("newest", "Cele mai noi", (q => q.OrderByDescending(d => d.CreatedAt)));

                    sortOptions.AddOption("popularity", "Cele mai populare", (q => q.OrderByDescending(d => d.OrderItems.Count)));

                    sortOptions.AddOption("most_rated", "Cele mai apreciate",
                        (q => q.OrderByDescending(d => d.DishRatings.Count).ThenByDescending(d => d.DishRatings.Average(dr => dr.Rating))));

                    sortOptions.AddOption("most_reviewed", "Cele mai discutate", (q => q.OrderByDescending(d => d.DishReviews.Count)));

                    sortOptions.AddOption("preparation_time", "Timp de preparare", (q => q.OrderBy(d => d.PreparationTime)));
                }

                return sortOptions;
            }
        }

        public DishesService(string userName) : base(userName, UserRole.CLIENT)
        {
            _userProfile = _unitOfWork.UserProfileRepository.Get(_id);
        }

        public IEnumerable<Dish> Get(string filterOption = null, string sortOption = null, int pageSize = 20, int pageNumber = 1)
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
                FilterOptions.GetFilterFunction(filterOption), SortOptions.GetSortFunction(sortOption))
                .Skip(pageNumber * pageSize).Take(pageSize);
        }

        public IEnumerable<Dish> GetByType(string dishType, string sortOption = null, int pageSize = 20, int pageNumber = 1)
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
                (d => (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                && (d.DishType.Name == dishType)),
                SortOptions.GetSortFunction(sortOption))
                .Skip(pageNumber * pageSize).Take(pageSize);
        }

        public IEnumerable<Dish> GetByRestaurant(int restaurantId, string dishType, int pageSize = 20, int pageNumber = 1)
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
                (d => (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                && (d.Restaurant.RestaurantId == restaurantId) && (d.DishType.Name == dishType)))
                .Skip(pageNumber * pageSize).Take(pageSize);
        }

        public int GetNumberOfPages(string filterOption = null, int pageSize = 20)
        {
            if (pageSize < 1)
            {
                pageSize = 20;
            }

            int numberOfDishes = _unitOfWork.DishRepository.Get(FilterOptions.GetFilterFunction(filterOption)).Count();

            return (int)Math.Ceiling((double)numberOfDishes / pageSize);
        }

        public IEnumerable<Dish> Search(string keywords, int pageSize = 20, int pageNumber = 1)
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
                d => ((d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City))
                && ((d.Name.Contains(keywords)) || (d.Description.Contains(keywords))))
                .Skip(pageNumber * pageSize).Take(pageSize);
        }

        public IEnumerable<Option> GetFilterOptions()
        {
            return FilterOptions.GetOptions();
        }

        public IEnumerable<Option> GetSortOptions()
        {
            return SortOptions.GetOptions();
        }

        public Dish Get(int id)
        {
            return _unitOfWork.DishRepository.Get(
                d => (d.DishId == id) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                ).FirstOrDefault();
        }

        public void Rate(DishRating dishRating)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
                d => (d.DishId == dishRating.DishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                ).FirstOrDefault();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishRating.DishId + " not found.");
            }

            DishRating existingDishRating = _unitOfWork.DishRatingRepository.Get(
                dr => (dr.UserProfileId == _userProfile.UserProfileId) && (dr.DishId == dish.DishId)
                ).FirstOrDefault();

            if (existingDishRating == null)
            {
                dishRating.CreatedAt = DateTime.Now;
                dishRating.UserProfileId = _userProfile.UserProfileId;

                _unitOfWork.DishRatingRepository.Add(dishRating);
            }
            else
            {
                existingDishRating.CreatedAt = DateTime.Now;
                existingDishRating.Rating = dishRating.Rating;

                _unitOfWork.DishRatingRepository.Update(existingDishRating);
            }

            _unitOfWork.Complete();
        }

        public DishRating GetMyRating(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
                 d => (d.DishId == dishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                 ).FirstOrDefault();

            if (dish == null)
            {
                return null;
            }

            return _unitOfWork.DishRatingRepository.Get(
                dr => (dr.UserProfileId == _userProfile.UserProfileId) && (dr.DishId == dish.DishId)
                ).FirstOrDefault();
        }

        public Nullable<double> GetAverageRating(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
                 d => (d.DishId == dishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                 ).FirstOrDefault();

            if (dish == null)
            {
                return null;
            }

            return _unitOfWork.DishRatingRepository.Get(
                dr => dr.DishId == dish.DishId
                ).Average(dr => dr.Rating);
        }

        public void AddReview(DishReview dishReview)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
                d => (d.DishId == dishReview.DishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                ).FirstOrDefault();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishReview.DishId + " not found.");
            }

            dishReview.CreatedAt = DateTime.Now;
            dishReview.UserProfileId = _userProfile.UserProfileId;

            _unitOfWork.DishReviewRepository.Add(dishReview);

            _unitOfWork.Complete();
        }

        public IEnumerable<DishReview> GetReviews(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
                d => (d.DishId == dishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
                ).FirstOrDefault();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            return _unitOfWork.DishReviewRepository.Get(
                 dr => dr.DishId == dish.DishId,
                q => q.OrderByDescending(dr => dr.CreatedAt),
                "UserProfile");
        }

        public IEnumerable<Allergen> GetAllergens(int dishId)
        {
            Dish dish = _unitOfWork.DishRepository.Get(
               d => (d.DishId == dishId) && (d.Restaurant.Country == _userProfile.Country) && (d.Restaurant.City == _userProfile.City)
               ).FirstOrDefault();

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            return dish.Allergens;
        }
    }
}
