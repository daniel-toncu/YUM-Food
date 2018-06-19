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
using DataAccess.Infrastructure.Authorization.Exceptions;
using Services.YUMServices.Clients;
using Services.YUMServices.Common;
using YUMFoodWebAPI.Models;

namespace YUMFoodWebAPI.Controllers
{
    [Authorize]
    public class ProfilesController : ApiController
    {
        private ProfilesService profilesService;
        private UsersService usersService;

        // GET: api/Profile
        [ResponseType(typeof(UserProfileDTO))]
        public IHttpActionResult GetUserProfile()
        {
            try
            {
                profilesService = new ProfilesService(User.Identity.Name);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }

            UserProfile userProfile = profilesService.Get();

            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                PhotoURL = userProfile.PhotoURL,
                Email = userProfile.Email,
                PhoneNumber = userProfile.PhoneNumber,
                Country = userProfile.Country,
                City = userProfile.City,
                Address = userProfile.Address
            };

            return Ok(userProfileDTO);
        }

        // POST: api/Profile
        [ResponseType(typeof(UserProfileDTO))]
        public IHttpActionResult PostUserProfile(UserProfileDTO userProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserProfile userProfile = new UserProfile()
            {
                FirstName = userProfileDTO.FirstName,
                LastName = userProfileDTO.LastName,
                PhotoURL = userProfileDTO.PhotoURL,
                Email = userProfileDTO.Email,
                PhoneNumber = userProfileDTO.PhoneNumber,
                Country = userProfileDTO.Country,
                City = userProfileDTO.City,
                Address = userProfileDTO.Address
            };

            try
            {
                profilesService = new ProfilesService(User.Identity.Name);
            }
            catch (Exception ex)
            {
                if ((ex is UserNotFoundException) || (ex is UserNotAuthorizedException))
                {
                    usersService = new UsersService();

                    usersService.RegisterClient(User.Identity.Name, userProfile);

                    return CreatedAtRoute("DefaultApi", new { id = userProfile.UserProfileId }, userProfileDTO);
                }
            }

            profilesService.Update(userProfile);

            return StatusCode(HttpStatusCode.NoContent);
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