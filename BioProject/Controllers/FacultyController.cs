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
    public class FacultyController : ApiController
    {
        private BiometricContextDb _context = new BiometricContextDb();

        public FacultyController() { }

        [HttpPost]
        [Route("faculties")]
        public IHttpActionResult addFaculty(FacultyDTO request)
        {
            try
            {
                if (UniversityHelper.isFacultyDataValid(request))
                {
                    var existUser = _context.faculties.SingleOrDefault(m => m.facaultyName == request.FacultyName 
                        && m.university.universityCode == request.UniversityCode);

                    if (existUser != null)
                    {
                        return BadRequest("Duplicate faulty name is not required: " + request.FacultyName);
                    }

                    var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == request.UniversityCode);

                    var newFaculty = new faculty()
                    {
                        facaultyName = request.FacultyName,
                        university = existUniversity
                    };
                    _context.faculties.Add(newFaculty);

                    _context.SaveChanges();

                    return Ok("successful");
                }
                else
                {
                    return BadRequest("Faculty name is required");
                }

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPut]
        [Route("faculties")]
        public IHttpActionResult updateFaculty(FacultyDTO request)
        {
            try
            {
                var existUser = _context.faculties.SingleOrDefault(m => m.id == request.Id);

                if (existUser == null)
                {
                    return BadRequest("Faculty does not exist.");
                }

                existUser.facaultyName = request.FacultyName != null ? request.FacultyName : existUser.facaultyName;

                _context.SaveChanges();

                return Ok("successful");

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("faculties")]
        public IHttpActionResult getFaculty(long facultyId)
        {
            try
            {
                var existUser = _context.faculties.SingleOrDefault(m => m.id == facultyId);


                if (existUser == null)
                {
                    return BadRequest("Faculty does not exist.");
                }

                var response = new FacultyResponseDTO()
                {
                    Id = existUser.id,
                    FacultyName = existUser.facaultyName
                };

                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("faculties/all")]
        public IHttpActionResult getAllFacultyByUniversityCode(string universityCode)
        {
            try
            {
                var existUniversity = _context.universities.SingleOrDefault(m => m.universityCode == universityCode
                      && m.activate == true);

                if (existUniversity == null)
                {
                    return BadRequest("University does not exist or has been deactivated");
                }

                var existAdminsQuery = _context.faculties.Where(m => m.university.universityCode == universityCode);

                var adminsList = existAdminsQuery.ToList();

                if (adminsList == null) return BadRequest("No faculties found");

                var admins = new List<FacultyResponseDTO>();
                foreach (var admin in adminsList)
                {
                    var data = new FacultyResponseDTO()
                    {
                        Id = admin.id,
                        FacultyName = admin.facaultyName
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

    }
}
