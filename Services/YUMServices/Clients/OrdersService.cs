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
    public class OrdersService : BaseAuthorizedService
    {
        protected OrdersService(string userName) : base(userName, UserRole.CLIENT)
        {
        }

        public IEnumerable<Order> Get()
        {
            return _unitOfWork.OrderRepository.Get(
                (o => o.UserProfileId == _id),
                (q => q.OrderByDescending(o => o.CreatedAt)),
                "OrderItem");
        }

        public Order Get(int orderId)
        {
            return _unitOfWork.OrderRepository.Get(
                (o => (o.UserProfileId == _id) && (o.OrderId == orderId))
                ).First();
        }

        public IEnumerable<OrderItem> GetOrderItems(int orderId)
        {
            Order order = _unitOfWork.OrderRepository.Get(
                (o => (o.UserProfileId == _id) && (o.OrderId == orderId))
                ).First();

            if (order == null)
            {
                throw new EntityNotFoundException("Order with id " + orderId + " not found.");
            }

            return order.OrderItems;
        }

        public OrderState GetOrderState(int orderId)
        {
            Order order = _unitOfWork.OrderRepository.Get(
              (o => (o.UserProfileId == _id) && (o.OrderId == orderId))
              ).First();

            if (order == null)
            {
                throw new EntityNotFoundException("Order with id " + orderId + " not found.");
            }

            return order.OrderState;
        }
    }
}
