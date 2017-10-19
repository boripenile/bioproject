using BioProject.Dtos;
using BioProject.Models;
using BioProject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BioProject.Controllers
{

    [RoutePrefix("biometric/api")]
    public class UserController : ApiController
    {
        private BiometricContextDb _context = new BiometricContextDb();

        public UserController() { }

        [HttpPost]
        [Route("admins")]
        public IHttpActionResult addUser(UniversityAdminDTO request)
        {
            try
            {
                if (UniversityHelper.isUniversityAdminrDataValid(request))
                {
                    var existUser = _context.users.SingleOrDefault(m => m.email == request.EmailAddress);

                    if (existUser != null)
                    {
                        return BadRequest("Email address has been taken: " + request.EmailAddress);
                    }

                    var newUser = new user()
                    {
                        email = request.EmailAddress,
                        password = request.Password,
                        universityCode = request.UniversityCode,
                        isAdmin = false
                    };
                    _context.users.Add(newUser);

                    _context.SaveChanges();

                    return Ok("successful");  
                }
                else
                {
                    return BadRequest("All fields are required.");
                }

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPut]
        [Route("admins")]
        public IHttpActionResult updateUser(UniversityAdminDTO request)
        {
            try
            {            
               var existUser = _context.users.SingleOrDefault(m => m.email == request.EmailAddress);

               if (existUser == null)
               {
                  return BadRequest("User does not exist.");
               }

               existUser.password = request.Password != null ? request.Password : existUser.password;
               
               _context.SaveChanges();

               return Ok("successful");

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("admins")]
        public IHttpActionResult getUser(long adminId)
        {
            try
            {
                var existUser = _context.users.SingleOrDefault(m => m.id == adminId);

                if (existUser == null)
                {
                    return BadRequest("User does not exist.");
                }

                var response = new UniversityAdminLiteResponseDTO()
                {
                    Id = existUser.id,
                    EmailAddress = existUser.email,
                    Password = existUser.password
                };

                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("admins/all")]
        public IHttpActionResult getAllUser(string universityCode)
        {
            try
            {
                var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode
                      && m.activate == true);

                if (existUniversity == null)
                {
                    return BadRequest("University does not exist or has been deactivated");
                }

                var existAdminsQuery = _context.users.Where(m => m.universityCode == universityCode);

                var adminsList = existAdminsQuery.ToList();

                if (adminsList == null) return BadRequest("No administrators found");

                var admins = new List<UniversityAdminLiteResponseDTO>();
                foreach (var admin in adminsList)
                {
                    var data = new UniversityAdminLiteResponseDTO()
                    {
                        Id = admin.id,
                        EmailAddress = admin.email,
                        Password = admin.password
                    };

                    admins.Add(data);

                }

                return Ok(admins);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("admins/university/all")]
        public IHttpActionResult getAllUser()
        {
            try
            {

                var existUniversityList = _context.universities.ToList();

                if (existUniversityList == null) return BadRequest("No Universities found");

                var universities = new List<UniversityAdminResponseDTO>();
                foreach (var univeristy in existUniversityList)
                {
                    var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == univeristy. universityCode
                     && m.activate == true);

                    var data = new UniversityAdminResponseDTO()
                    {
                        Id = univeristy.id,
                        Activated = existUniversity.activate,
                        UniversityAddress = existUniversity.universityAddress,
                        UniversityCode = existUniversity.universityCode,
                        UniversityName = existUniversity.universityName,
                        EmailAddress = existUniversity.email,
                        PhoneNumber = existUniversity.mobile
                    };

                    universities.Add(data);

                }

                return Ok(universities);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }
    }
}
