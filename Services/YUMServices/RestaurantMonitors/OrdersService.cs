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

namespace Services.YUMServices.RestaurantMonitors
{
    public class OrdersService : BaseAuthorizedService
    {
        protected OrdersService(string userName) : base(userName, UserRole.RESTAURANT_MONITOR)
        {
        }

        public IEnumerable<Order> Get()
        {
            return _unitOfWork.OrderRepository.Get(
                (o => o.RestaurantId == _id),
                (q => q.OrderBy(o => o.OrderStateId).ThenBy(o => o.CreatedAt)));
        }

        public Order Get(int orderId)
        {
            return _unitOfWork.OrderRepository.Get(
                (o => (o.RestaurantId == _id) && (o.OrderId == orderId))
                ).First();
        }

        public IEnumerable<OrderItem> GetOrderItems(int orderId)
        {
            Order order = _unitOfWork.OrderRepository.Get(
                (o => (o.RestaurantId == _id) && (o.OrderId == orderId))
                ).First();

            if (order == null)
            {
                throw new EntityNotFoundException("Order with id " + orderId + " not found.");
            }

            return order.OrderItems;
        }

        public void UpdateOrderState(int orderId, int orderStateId)
        {
            Order order = _unitOfWork.OrderRepository.Get(
                (o => (o.RestaurantId == _id) && (o.OrderId == orderId))
                ).First();

            if (order == null)
            {
                throw new EntityNotFoundException("Order with id " + orderId + " not found.");
            }

            OrderState orderState = _unitOfWork.OrderStateRepository.Get(orderStateId);

            if (orderState == null)
            {
                throw new EntityNotFoundException("OrderState with id " + orderStateId + " not found.");
            }

            order.OrderState = orderState;

            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Complete();
        }
    }
}
