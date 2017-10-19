using BioProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Models
{
    public class UniversityHelper
    {
        public static bool isDepartmentDataValid(DepartmentDTO departmentDTO)
        {

            if (String.IsNullOrWhiteSpace(departmentDTO.DepartmentName))
            {
                return false;
            }

            return true;
        }
        public static bool isFacultyDataValid(FacultyDTO facultDTO)
        {

            if (String.IsNullOrWhiteSpace(facultDTO.UniversityCode) ||
                String.IsNullOrWhiteSpace(facultDTO.FacultyName))
            {
                return false;
            }

            return true;
        }
        public static bool isUniversityAdminrDataValid(UniversityAdminDTO adminDTO)
        {

            if (String.IsNullOrWhiteSpace(adminDTO.UniversityCode) ||
                String.IsNullOrWhiteSpace(adminDTO.EmailAddress) ||
                String.IsNullOrWhiteSpace(adminDTO.Password))
            {
                return false;
            }

            return true;
        }
        public static bool isFingerDataValid(FingerPrintDTO fingerDTO)
        {

            if (String.IsNullOrWhiteSpace(fingerDTO.LeftThumb.FingerImage) ||
                String.IsNullOrWhiteSpace(fingerDTO.RightThumb.FingerImage))
            {
                return false;
            }

            return true;
        }
        public static bool isStudentDataValid(StudentDTO studentDTO)
        {

            if (String.IsNullOrWhiteSpace(studentDTO.MatricNumber) ||
                String.IsNullOrWhiteSpace(studentDTO.FirstName) ||
                String.IsNullOrWhiteSpace(studentDTO.Surname) ||
                String.IsNullOrWhiteSpace(studentDTO.Email) ||
                studentDTO.DepartmentId == 0)
            {
                return false;
            }

            return true;
        }

        public static bool isUniversityDataValid(UniversityDTO university) {

            if (String.IsNullOrWhiteSpace(university.EmailAddress) ||
                String.IsNullOrWhiteSpace(university.PhoneNumber) ||
                String.IsNullOrWhiteSpace(university.UniversityName) ||
                String.IsNullOrWhiteSpace(university.UniversityAddress) ||
                String.IsNullOrWhiteSpace(university.Password))
            {
                return false;
            }

            return true;
        }

        public static bool isActivationDataValid(ActivationDTO activation)
        {

            if (String.IsNullOrWhiteSpace(activation.EmailAddress) ||
                String.IsNullOrWhiteSpace(activation.UniversityCode))
            {
                return false;
            }

            return true;
        }

        public static bool isLoginDataValid(LoginDTO login)
        {

            if (String.IsNullOrWhiteSpace(login.EmailAddress) ||
                String.IsNullOrWhiteSpace(login.Password))
            {
                return false;
            }

            return true;
        }
    }
}