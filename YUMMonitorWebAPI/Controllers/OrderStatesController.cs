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
using DataAccess.Model;

namespace YUMMonitorWebAPI.Controllers
{
    public class OrderStatesController : ApiController
    {
        private YUMFoodEntities db = new YUMFoodEntities();

        // GET: api/OrderStates
        public IQueryable<OrderState> GetOrderStates()
        {
            return db.OrderStates;
        }

        // GET: api/OrderStates/5
        [ResponseType(typeof(OrderState))]
        public IHttpActionResult GetOrderState(int id)
        {
            OrderState orderState = db.OrderStates.Find(id);
            if (orderState == null)
            {
                return NotFound();
            }

            return Ok(orderState);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderStateExists(int id)
        {
            return db.OrderStates.Count(e => e.OrderStateId == id) > 0;
        }
    }
}