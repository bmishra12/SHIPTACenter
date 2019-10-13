using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UmbracoShipTac.Models
{
    public class ProfileViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int MemberID { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Remote("CheckEmailIsUsed", "ProfileSurface", ErrorMessage = "The email address has already been registered")]
        public string EmailAddress { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string MemberType { get; set; }

        [Required]
        [Remote("CheckProfileURLAvailable", "ProfileSurface", ErrorMessage = "The profile URL is already in use")]
        public string profileUrl { get; set; }

        public string Description { get; set; }


    }
}