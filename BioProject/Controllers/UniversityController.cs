using BioProject.Dtos;
using BioProject.Helper;
using BioProject.Models;
using BioProject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BioProject.Controllers
{
    [RoutePrefix("biometric/api/universities")]
    public class UniversityController : ApiController
    {
        private BiometricContextDb _context = new BiometricContextDb();

        public UniversityController() { }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUniversity(LoginDTO request)
        {
            try
            {
                if (UniversityHelper.isLoginDataValid(request))
                {
                    var existUser = _context.users.SingleOrDefault(m => m.email == request.EmailAddress
                        && m.password == request.Password);
                    if (existUser == null) return BadRequest("Invalid Email address or password.");

                    var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == existUser.universityCode
                       && m.activate == true);

                    if (existUniversity == null) return BadRequest("Invalid user credentials for university admin");

                    var response = new UniversityAdminResponseDTO()
                    {
                        Id = existUser.id,
                        Activated = existUniversity.activate,
                        UniversityAddress = existUniversity.universityAddress,
                        UniversityCode = existUniversity.universityCode,
                        UniversityName = existUniversity.universityName,
                        EmailAddress = existUser.email,
                        PhoneNumber = existUniversity.mobile
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("Email address and registered password are required.");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPost]
        [Route("activate")]
        public async  Task<IHttpActionResult> activateUniversity(ActivationDTO request)
        {
            try
            {
                if (UniversityHelper.isActivationDataValid(request))
                {
                    var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == request.UniversityCode
                        && m.email == request.EmailAddress && m.activate == false);
                    if (existUniversity == null) return BadRequest("Invalid University code or email address.");

                    existUniversity.activate = true;

                    await _context.SaveChangesAsync();

                    var response = new ActivationResponseDTO()
                    {
                        Activated = existUniversity.activate,
                        UniversityAddress = existUniversity.universityAddress,
                        UniversityCode = existUniversity.universityCode,
                        UniversityName = existUniversity.universityName,
                        EmailAddress = existUniversity.email,
                        PhoneNumber = existUniversity.mobile
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("University code and registered email address are required.");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> createUniversity(UniversityDTO request)
        {
            try
            {
                if (UniversityHelper.isUniversityDataValid(request))
                {
                    var existUser = _context.users.SingleOrDefault(m => m.email == request.EmailAddress);

                    if (!String.IsNullOrWhiteSpace(existUser.email))
                        return BadRequest("A university already registered with the email address: " + existUser.email);

                    String newUniversityCode = getUniversityCode();

                    var newUser = new user()
                    {
                        email = request.EmailAddress,
                        isAdmin = false,
                        password = request.Password
                    };

                    _context.users.Add(newUser);

                    await _context.SaveChangesAsync();

                    var newUniversity = new university()
                    {
                        mobile = request.PhoneNumber,
                        universityAddress = request.UniversityAddress != null ? request.UniversityAddress : String.Empty,
                        universityCode = newUniversityCode,
                        email = newUser.email,
                        password = newUser.password,
                        insertedBy = newUser.email,
                        universityName = request.UniversityName,
                        insertedDate = new DateTime()

                    };

                    _context.universities.Add(newUniversity);

                    await _context.SaveChangesAsync();

                    var response = new RegisterResponseDTO()
                    {
                        EmailAddress = newUser.email,
                        UniversityCode = newUniversity.universityCode
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("Please provide required fields.");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        private String getUniversityCode()
        {
            var universityCode = Utils.GenerateDigits(7);

            var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode);

            while (!String.IsNullOrWhiteSpace(existUniversity.universityCode))
            {
                universityCode = Utils.GenerateDigits(7);

                existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode);
            }

            return universityCode;
        }

    }

}
