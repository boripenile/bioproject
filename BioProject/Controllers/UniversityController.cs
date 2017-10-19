using BioProject.Dtos;
using BioProject.Helper;
using BioProject.Models;
using BioProject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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

                    if (existUser.universityCode != null)
                    {
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
                            PhoneNumber = existUniversity.mobile,
                            Super = existUser.isAdmin.GetValueOrDefault()
                        };

                        return Ok(response);
                    }
                    else
                    {
                        var superAdmin = new UniversityAdminLiteResponseDTO()
                        {
                            Id = existUser.id,
                            EmailAddress = existUser.email,
                            Super = existUser.isAdmin.GetValueOrDefault()
                        };

                        return Ok(superAdmin);
                    }
                    
                }
                else
                {
                    return BadRequest("Email address and password are required.");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("activate")]
        public async Task<IHttpActionResult> activateUniversity(string universityCode)
        {
            try
            {
               
                    var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode
                        && m.activate == false);
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
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult createUniversity(UniversityDTO request, string email)
        {
            try
            {
                if (UniversityHelper.isUniversityDataValid(request))
                {
                    var existAdmin = _context.users.SingleOrDefault(m => m.email == email && m.isAdmin == true);

                    if (existAdmin == null)
                        return BadRequest("Invalid user");

                    var existUser = _context.users.SingleOrDefault(m => m.email == request.EmailAddress);

                    if (existUser != null)
                        return BadRequest("A university already registered with the email address: " + existUser.email);

                    String newUniversityCode = getUniversityCode();

                    var newUser = new user()
                    {
                        email = request.EmailAddress,
                        isAdmin = true,
                        password = request.Password,
                        universityCode = newUniversityCode
                    };

                    _context.users.Add(newUser);
                    var newUniversity = new university()
                    {
                        mobile = request.PhoneNumber,
                        universityAddress = request.UniversityAddress != null ? request.UniversityAddress : String.Empty,
                        universityCode = newUniversityCode,
                        email = newUser.email,
                        password = newUser.password,
                        insertedBy = existAdmin.email,
                        universityName = request.UniversityName,
                        insertedDate = new DateTime()

                    };

                    _context.universities.Add(newUniversity);

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.To.Add(newUser.email);
                    mailMessage.From = new MailAddress(email);
                    mailMessage.Subject = "Activate your Registration";
                    var subBody = "<h6>Click the following link or copy and paste in a browser to activate your registration.</h6><br/><br/>";
                    mailMessage.Body = subBody + "http://localhost:8080/activate/" + newUser.universityCode;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                                     
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("alitestaps@gmail.com", "@SeconD2");
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Someting went wrong: " + ex.Message);
                    }
                    var response = new RegisterResponseDTO()
                    {
                        EmailAddress = newUser.email,
                        UniversityCode = newUniversity.universityCode
                    };

                    _context.SaveChangesAsync();
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

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUniversities(string email)
        {
            try
            {
                var existUser = _context.users.SingleOrDefault(m => m.email == email
                      && m.isAdmin == true);

                if (existUser == null)
                {
                    return BadRequest("User is not a super admin");
                }

                var existUniversitiesQuery = _context.universities;

                var universityList = existUniversitiesQuery.ToList();

                if (universityList == null) return BadRequest("No university found");

                var universities = new List<UniversityAdminResponseDTO>();
                foreach (var university in universityList)
                {
                    var data = new UniversityAdminResponseDTO()
                    {
                        Id = university.id,
                        Activated = university.activate,
                        EmailAddress = university.email,
                        PhoneNumber = university.mobile,
                        Super = false,
                        UniversityAddress = university.universityAddress,
                        UniversityCode = university.universityCode,
                        UniversityName = university.universityName
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
        private String getUniversityCode()
        {
            var universityCode = Utils.GenerateDigits(7);

            var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode);

            while (existUniversity != null)
            {
                universityCode = Utils.GenerateDigits(7);

                existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode);
            }

            return universityCode;
        }

    }

}
