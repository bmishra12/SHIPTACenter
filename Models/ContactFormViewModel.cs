using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UmbracoShipTac.Models
{
    public class ContactFormViewModel
    {

        
        [Required]
        [DisplayName("First Name")]
        public string FName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LName { get; set; }

        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Email Confirm")]
        [Compare("Email", ErrorMessage = "The Email and confirmation Email do not match.")]
        public string CEmail { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Address 1")]
        public string Address1 { get; set; }

        [DisplayName("Address 2")]
        public string Address2 { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Origin")]
        public string Origin { get; set; }

        [DisplayName("Zip")]
        public string Zip { get; set; }

        [DisplayName("Your Phone")]
        public string Phone { get; set; }

        [Required]
        [DisplayName("Comments")]
        public string Comments { get; set; }

        public int RedirectPage { get; set; }
    }
}