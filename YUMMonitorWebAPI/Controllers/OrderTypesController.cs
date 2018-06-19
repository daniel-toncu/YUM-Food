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
    public class OrderTypesController : ApiController
    {
        private YUMFoodEntities db = new YUMFoodEntities();

        // GET: api/OrderTypes
        public IQueryable<OrderType> GetOrderTypes()
        {
            return db.OrderTypes;
        }

        // GET: api/OrderTypes/5
        [ResponseType(typeof(OrderType))]
        public IHttpActionResult GetOrderType(int id)
        {
            OrderType orderType = db.OrderTypes.Find(id);
            if (orderType == null)
            {
                return NotFound();
            }

            return Ok(orderType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderTypeExists(int id)
        {
            return db.OrderTypes.Count(e => e.OrderTypeId == id) > 0;
        }
    }
}