using DataAccess.Infrastructure.Authorization;
using DataAccess.Model;
using Services.YUMServices.Base;
using Services.YUMServices.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Infrastructure.Authorization.AuthorizationManager;

namespace Services.YUMServices.Clients
{
    public class ShoppingCartsService : BaseAuthorizedService
    {
        protected ShoppingCartsService(string userName) : base(userName, UserRole.CLIENT)
        {
        }

        public IEnumerable<ShoppingCart> Get()
        {
            return _unitOfWork.ShoppingCartRepository.Get(
                (sc => sc.UserProfileId == _id),
                (q => q.OrderBy(sc => sc.Restaurant.RestaurantId).ThenByDescending(sc => sc.AddedAt)));
        }

        public float GetRestaurantSubTotal(int restaurantId)
        {
            IEnumerable<ShoppingCart> shoppingCartItems = _unitOfWork.ShoppingCartRepository.Get(
                  (sc => (sc.UserProfileId == _id) && (sc.RestaurantId == restaurantId)));

            float total = 0;
            foreach (ShoppingCart shoppingCart in shoppingCartItems)
            {
                total += shoppingCart.Dish.Price * shoppingCart.Quantity;
            }

            return total;
        }

        public float GetShoppingCartSubTotal(int shoppingCartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCartRepository.Get(shoppingCartId);

            return shoppingCart.Dish.Price * shoppingCart.Quantity;
        }

        public float GetTotal()
        {
            IEnumerable<ShoppingCart> shoppingCartItems = _unitOfWork.ShoppingCartRepository.Get(
                (sc => sc.UserProfileId == _id));

            float total = 0;
            foreach (ShoppingCart shoppingCart in shoppingCartItems)
            {
                total += shoppingCart.Dish.Price * shoppingCart.Quantity;
            }

            return total;
        }

        public void CompletePurchase(Nullable<DateTime> scheduledDelivery = null, Nullable<bool> bookTable = null)
        {
            IEnumerable<Restaurant> restaurants = _unitOfWork.ShoppingCartRepository
                .Query()
                .Where(sc => sc.UserProfileId == _id)
                .Select(sc => sc.Restaurant)
                .Distinct();

            OrderState orderState = _unitOfWork.OrderStateRepository
                .Get((os => os.Name == "UNPROCESSED")).First();
            OrderType orderType = _unitOfWork.OrderTypeRepository
                .Get(ot => ot.Name == "DELIVERY").First();

            foreach (Restaurant restaurant in restaurants)
            {
                Order order = new Order()
                {
                    CreatedAt = DateTime.Now,
                    UserProfileId = _id,
                    Restaurant = restaurant,
                    OrderState = orderState,
                    OrderType = orderType,
                    ScheduledDelivery = scheduledDelivery,
                    BookTable = bookTable
                };

                if (restaurant.MinOrderFreeDelivery > GetRestaurantSubTotal(restaurant.RestaurantId))
                {
                    order.DeliveryPrice = 0;
                }
                else
                {
                    order.DeliveryPrice = restaurant.DeliveryPrice;
                }

                _unitOfWork.OrderRepository.Add(order);

                IEnumerable<ShoppingCart> shoppingCartItems = _unitOfWork.ShoppingCartRepository
                    .Get(sc => (sc.UserProfileId == _id) && (sc.RestaurantId == restaurant.RestaurantId));

                foreach (ShoppingCart shoppingCart in shoppingCartItems)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        Order = order,
                        Dish = shoppingCart.Dish,
                        Quantity = shoppingCart.Quantity
                    };

                    _unitOfWork.ShoppingCartRepository.Remove(shoppingCart);
                    _unitOfWork.OrderItemRepository.Add(orderItem);
                }
            }

            _unitOfWork.Complete();
        }

        public void AddToShoppingCart(int dishId, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            Dish dish = _unitOfWork.DishRepository.Get(dishId);

            if (dish == null)
            {
                throw new EntityNotFoundException("Dish with id " + dishId + " not found.");
            }

            ShoppingCart shoppingCart = _unitOfWork.ShoppingCartRepository.Get(
                sc => (sc.UserProfileId == _id) && (sc.DishId == dishId)).First();

            if (shoppingCart == null)
            {
                if (quantity < 1)
                {
                    return;
                }

                shoppingCart = new ShoppingCart()
                {
                    UserProfileId = _id,
                    AddedAt = DateTime.Now,
                    DishId = dishId,
                    Quantity = quantity,
                    RestaurantId = dish.RestaurantId
                };

                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Complete();
            }
            else
            {
                shoppingCart.Quantity += quantity;
            }
        }
    }
}
