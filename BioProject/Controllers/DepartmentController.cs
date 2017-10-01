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
    public class DepartmentController : ApiController
    {
        private BiometricContextDb _context = new BiometricContextDb();

        public DepartmentController() { }

        [HttpPost]
        [Route("departments")]
        public IHttpActionResult addDepartment(DepartmentDTO request)
        {
            try
            {
                if (UniversityHelper.isDepartmentDataValid(request))
                {
                  
                    var existFaculty = _context.faculties.SingleOrDefault(m => m.id == request.FacultyId);

                    if (existFaculty == null) return BadRequest("Invalid faculty");

                    var existDepartment = _context.departments.SingleOrDefault(m => m.departmentName == request.DepartmentName
                       && m.faculty.id == request.FacultyId);

                    if (existDepartment != null)
                    {
                        return BadRequest("Duplicate department name is not required: " + request.DepartmentName);
                    }
                   
                    var newDepartment = new department()
                    {
                        faculty = existFaculty,
                        departmentName = request.DepartmentName
                    };
                    _context.departments.Add(newDepartment);

                    _context.SaveChanges();

                    return Ok("successful");
                }
                else
                {
                    return BadRequest("Department name is required");
                }

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPut]
        [Route("departments")]
        public IHttpActionResult updateDepartment(DepartmentDTO request)
        {
            try
            {
                var existUser = _context.departments.SingleOrDefault(m => m.id == request.Id);

                if (existUser == null)
                {
                    return BadRequest("Department does not exist.");
                }

                existUser.departmentName = request.DepartmentName != null ? request.DepartmentName : existUser.departmentName;
               
                var existDepartment = _context.departments.SingleOrDefault(m => m.departmentName == existUser.departmentName
                      && m.faculty.id == existUser.faculty.id);

                if (existDepartment != null)
                {
                    return BadRequest("Duplicate department name is not required: " + request.DepartmentName);
                }

                _context.SaveChanges();

                return Ok("successful");

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("departments")]
        public IHttpActionResult getDepartment(long departmentId)
        {
            try
            {
                var existUser = _context.departments.SingleOrDefault(m => m.id == departmentId);


                if (existUser == null)
                {
                    return BadRequest("Department does not exist.");
                }

                var response = new DepartmentResponseDTO()
                {
                    Id = existUser.id,
                    DepartmentName = existUser.departmentName
                };

                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("departments")]
        public IHttpActionResult getAlDepartmentsByFaculty(long facultyId)
        {
            try
            {
                var existUniversity = _context.faculties.SingleOrDefault(m => m.id == facultyId);

                if (existUniversity == null)
                {
                    return BadRequest("Faculty does not exist or has been deactivated");
                }

                var existAdminsQuery = _context.departments.Where(m => m.faculty.id == existUniversity.id);

                var departmentsList = existAdminsQuery.ToList();

                if (departmentsList == null) return BadRequest("No department found");

                var departments = new List<DepartmentResponseDTO>();
                foreach (var department in departmentsList)
                {
                    var data = new DepartmentResponseDTO()
                    {
                        Id = department.id,
                        DepartmentName = department.departmentName
                    };

                    departments.Add(data);

                }

                return Ok(departments);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }
    }
}
