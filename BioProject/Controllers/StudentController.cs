﻿using BioProject.Dtos;
using BioProject.Models;
using BioProject.Response;
using GriauleFingerprintLibrary.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BioProject.Controllers
{
    [RoutePrefix("biometric/api/students")]
    public class StudentController : ApiController
    {
        private BiometricContextDb _context = new BiometricContextDb();

        public StudentController() { }

        [HttpPost]
        [Route("verification")]
        public IHttpActionResult verifyStudent(VerifyFingerDTO finger)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(finger.AnyFingerImage)) return BadRequest("Image of index or thumb of either right or left finger is required.");

                Int64 studentId = VerifyFingerPrint(finger);

                if (studentId > 0)
                {
                    var student = _context.students.SingleOrDefault(m => m.id == studentId);

                    var picture = _context.pictures.SingleOrDefault(m => m.student.id == studentId);

                    if (student != null && picture != null)
                    {
                        var response = new VerifiedStudentResponseDTO()
                        {
                            StudentId = student.id,
                            Department = student.department.departmentName,
                            Faculty = student.department.faculty.facaultyName,
                            Email = student.email,
                            FirstName = student.firstName,
                            Surname = student.surname,
                            OtherName = student.otherName != null ? student.otherName : "",
                            MatricNumber = student.matricNumber,
                            UniversityName = student.department.faculty.university.universityName,
                            Status = student.status,
                            Picture = Convert.ToBase64String(picture.image)
                        };

                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("Student can not be verified");
                    }
                   
                }
                else
                {
                    return BadRequest("Student can not be verified");
                }
            }
            catch (Exception e)
            {
                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpGet]
        [Route("university")]
        public IHttpActionResult fetchStudentsByUniversityCode(String universityCode)
        {
            try
            {
                var existStudentsQuery = _context.students.Where(m => m.department.faculty.university.universityCode == universityCode);

                var studentsList = existStudentsQuery.ToList();

                if (studentsList == null)
                {
                    return BadRequest("No students found for the university code: " + universityCode);
                }

                var studentsResponse = new List<StudentResponseDTO>();
                foreach (var student in studentsList)
                {
                    var data = new StudentResponseDTO()
                    {
                        Department = student.department.departmentName,
                        Faculty = student.department.faculty.facaultyName,
                        Email = student.email,
                        FirstName = student.firstName,
                        Surname = student.surname,
                        OtherName = student.otherName != null ? student.otherName : "",
                        MatricNumber = student.matricNumber,
                        UniversityName = student.department.faculty.university.universityName,
                        Status = student.status
                    };
                    studentsResponse.Add(data);
                }
                return Ok(studentsResponse);
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
      
        }

        [HttpGet]
        [Route("university/department")]
        public IHttpActionResult fetchStudentsByUniversityCode(long departmentId)
        {
            try
            {
                var existStudentsQuery = _context.students.Where(m => m.department.id == departmentId);

                var studentsList = existStudentsQuery.ToList();

                if (studentsList == null)
                {
                    return BadRequest("No students found for the department id: " + departmentId);
                }

                var studentsResponse = new List<StudentResponseDTO>();
                foreach (var student in studentsList)
                {
                    var data = new StudentResponseDTO()
                    {
                        Department = student.department.departmentName,
                        Faculty = student.department.faculty.facaultyName,
                        Email = student.email,
                        FirstName = student.firstName,
                        Surname = student.surname,
                        OtherName = student.otherName != null ? student.otherName : "",
                        MatricNumber = student.matricNumber,
                        UniversityName = student.department.faculty.university.universityName,
                        Status = student.status
                    };
                    studentsResponse.Add(data);
                }
                return Ok(studentsResponse);
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }

        }

        [HttpPost]
        [Route("enrol")]
        public IHttpActionResult enrolStudent(StudentRegistrationDTO request)
        {
            try
            {
                if (UniversityHelper.isStudentDataValid(request.Biodata))
                {
                    System.Diagnostics.Debug.WriteLine("Started");
                    if (UniversityHelper.isFingerDataValid(request.Biometric) && !String.IsNullOrWhiteSpace(request.Picture))
                    {
                        System.Diagnostics.Debug.WriteLine("Valid data");
                        var existDepartment = _context.departments.SingleOrDefault(m => m.id == request.Biodata.DepartmentId);
                        if (existDepartment == null) return BadRequest("Invalid department.");

                        System.Diagnostics.Debug.WriteLine("Valid department.");

                        var existMatricByUniversityCode = _context.students.SingleOrDefault(m => m.matricNumber == request.Biodata.MatricNumber
                            && m.department.faculty.university.universityCode == existDepartment.faculty.university.universityCode);
                        if (existMatricByUniversityCode != null) return BadRequest("Duplicate Matric number is not allows");

                        System.Diagnostics.Debug.WriteLine("Valid matric number.");
                        var newStudent = new student()
                        {
                            department = existDepartment,
                            email = request.Biodata.Email,
                            firstName = request.Biodata.FirstName,
                            surname = request.Biodata.Surname,
                            otherName = request.Biodata.OtherName != null ? request.Biodata.OtherName : "",
                            matricNumber = request.Biodata.MatricNumber,
                            insertedBy = request.Biodata.Email,
                            insertedDate = DateTime.Now,
                            status = true
                        };

                        _context.students.Add(newStudent);
                        _context.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Data saved.");

                        
                        var newFingerPrints = new fingerprint()
                        {
                            student = newStudent,
                            //leftIndex = Convert.FromBase64String(request.Biometric.LeftIndex),
                            leftThumb = Convert.FromBase64String(request.Biometric.LeftThumb.FingerImage),
                            //rightIndex = Convert.FromBase64String(request.Biometric.RightIndex),
                            rightThumb = Convert.FromBase64String(request.Biometric.RightThumb.FingerImage)
                        };

                        _context.fingerprints.Add(newFingerPrints);
                        _context.SaveChanges();

                        System.Diagnostics.Debug.WriteLine("Fingerprints saved.");

                        

                        if (request.Template.LeftThumb != null && request.Template.RightThumb != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Template 123");
                            var temp1 = new fingertemplate()
                                {
                                    student = newStudent,
                                    template = Convert.FromBase64String(request.Template.LeftThumb),
                                    quality = 1
                                };
                                _context.fingertemplates.Add(temp1);
                                _context.SaveChanges();
                            var temp2 = new fingertemplate()
                                {
                                    student = newStudent,
                                    template = Convert.FromBase64String(request.Template.RightThumb),
                                    quality = 1
                                };
                                _context.fingertemplates.Add(temp2);
                                _context.SaveChanges();
                                System.Diagnostics.Debug.WriteLine("Template 234");
                        }

                        else
                        {
                            //SaveFingerprintTemplate(request.Biometric.LeftIndex, newStudent);
                            SaveFingerprintTemplate(request.Biometric.LeftThumb, newStudent);
                            //SaveFingerprintTemplate(request.Biometric.RightIndex, newStudent);
                            SaveFingerprintTemplate(request.Biometric.RightThumb, newStudent);
                        }
                        var newPicture = new picture()
                        {
                            image = Convert.FromBase64String(request.Picture),
                            student = newStudent
                        };

                        _context.pictures.Add(newPicture);

                        _context.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Save record saved.");
                        return Ok(newStudent.matricNumber);
                }
                else
                {
                    return BadRequest("Student biometric and picture are required");
                }
            }
                else
                {
                    return BadRequest("Student biodata is required");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult updateStudent(StudentDTO request)
        {
            try
            {
                var existMatricByMaticNumber = _context.students.SingleOrDefault(m => m.matricNumber == request.MatricNumber);
                if (existMatricByMaticNumber == null) return BadRequest("Invalid student matric number");

                if(request.DepartmentId > 0)
                {
                    var existDepartment = _context.departments.SingleOrDefault(m => m.id == request.DepartmentId);
                    if (existDepartment == null) return BadRequest("Invalid department.");
                    existMatricByMaticNumber.department = existDepartment;
                }
                existMatricByMaticNumber.email = request.Email != null ? request.Email : existMatricByMaticNumber.email;
                existMatricByMaticNumber.firstName = request.FirstName != null ? request.FirstName : existMatricByMaticNumber.firstName;
                existMatricByMaticNumber.otherName = request.OtherName != null ? request.OtherName : existMatricByMaticNumber.otherName;
                existMatricByMaticNumber.surname = request.Surname != null ? request.Surname : existMatricByMaticNumber.surname;
                existMatricByMaticNumber.status = request.Status != null ? request.Status.GetValueOrDefault() : existMatricByMaticNumber.status;
                existMatricByMaticNumber.updatedDate = DateTime.Now;

                _context.SaveChanges();

                var response = new StudentResponseDTO()
                {
                    Department = existMatricByMaticNumber.department.departmentName,
                    Faculty = existMatricByMaticNumber.department.faculty.facaultyName,
                    Email = existMatricByMaticNumber.email,
                    FirstName = existMatricByMaticNumber.firstName,
                    Surname = existMatricByMaticNumber.surname,
                    OtherName = existMatricByMaticNumber.otherName,
                    MatricNumber = existMatricByMaticNumber.matricNumber,
                    UniversityName = existMatricByMaticNumber.department.faculty.university.universityName,
                    Status = existMatricByMaticNumber.status
                };

                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest("Something went wrong: " + e.Message);
            }
        }
        private Int64 VerifyFingerPrint(VerifyFingerDTO fingerPrint)
        {
            try
            {
                var onlineTemplates = _context.fingertemplates.ToList();
                if (onlineTemplates == null) return 0;
                // FingerprintTemplate templateToVerify = ConvertFingerToTemplate(fingerPrint);
                var templateToVerify = new fingertemplate()
                {
                    quality = 1,
                    template = Convert.FromBase64String(fingerPrint.AnyFingerImage)
                };

                var userTemplate = ConvertToTemplateFromDb(templateToVerify);

                foreach (var template in onlineTemplates)
                {
                    var dbTemplate = ConvertToTemplateFromDb(template);
                    int score = 0;
                    if (Verify(dbTemplate, userTemplate, out score))
                    {
                        return template.student.id;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Something went wrong: " + e.Message);
                return 0;
            }

            return 0;
        }

        private FingerprintTemplate ConvertToTemplateFromDb(fingertemplate templateDb)
        {
            byte[] buff = templateDb.template;
            int quality = templateDb.quality;

            var _testTemplateT = new GriauleFingerprintLibrary.DataTypes.FingerprintTemplate()
            {
                Size = buff.Length,
                Buffer = buff,
                Quality = quality
            };

            return _testTemplateT;
        }

        private FingerprintTemplate ConvertFingerToTemplate(FingerPrint finger)
        {
            var imageRaw = FingerPrintHelper.Instance.GetRawImageFromByte(finger);

            if (imageRaw != null)
            {
                return FingerPrintHelper.Instance.ConvertRawImageToTemplate(imageRaw);
            }
            return null;
        }

        public static bool Verify(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate queryTemplate,
           GriauleFingerprintLibrary.DataTypes.FingerprintTemplate referenceTemplate, out int verifyScore)
        {
            return FingerPrintHelper.Instance.RefFingercore.Verify(queryTemplate, referenceTemplate, out verifyScore) ==
                   1
                ? true
                : false;
        }

        private string GetTemplateFromFingerString(FingerPrint finger)
        {
            var imageRaw = FingerPrintHelper.Instance.GetRawImageFromByte(finger);

            GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _templ = null;

            FingerPrintHelper.Instance.RefFingercore.Extract(imageRaw, ref _templ);

            return Convert.ToBase64String(_templ.Buffer);
        }

        private void SaveFingerprintTemplate(FingerPrint fingerPrint, student student)
        {
            var imageRaw = FingerPrintHelper.Instance.GetRawImageFromByte(fingerPrint);

            GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _templ = null;

            System.Diagnostics.Debug.WriteLine("Here 999");

            FingerPrintHelper.Instance.RefFingercore.Extract(imageRaw, ref _templ);

            System.Diagnostics.Debug.WriteLine("Here 888");

            System.Diagnostics.Debug.WriteLine("Template: " + Convert.ToBase64String(_templ.Buffer));

            var fTemplate = new fingertemplate()
            {
                student = student,//
                template = _templ.Buffer,
                quality = _templ.Quality
            };

            _context.fingertemplates.Add(fTemplate);
            _context.SaveChanges();
            System.Diagnostics.Debug.WriteLine("Fingerprint template 1 saved.");
        }
    }
}
