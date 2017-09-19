using BioProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Models
{
    public class UniversityHelper
    {
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