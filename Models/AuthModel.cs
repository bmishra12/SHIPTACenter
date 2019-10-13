
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;



namespace UmbracoShipTac.Models
{
    /// <summary>
    /// Register View Model
    /// </summary>
    public class RegisterViewModel
    {

        public string Name { get; set; }

        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Role name is required")]
        public string Role { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State name is required")]
        public string State { get; set; }


        [DisplayName("City")]
        [Required(ErrorMessage = "City name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string City { get; set; }


        [DisplayName("Organization ")]
        [Required(ErrorMessage = "Organization  name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string Organization { get; set; }

        //[DisplayName("County")]
        //   [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
        //    ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        //public string County { get; set; }

        [DisplayName("Reason for this request ")]
        [Required(ErrorMessage = "Reason  for this request  is required")]
        [RegularExpression(@"^[a-zA-Z0-9\.,\n'\-.\s]{1,200}$",
            ErrorMessage = "Only alphabets, numbers, apostrophe, hyphens, comma and period allowed; maximum 200 characters.")]
        public string Reason { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string LastName { get; set; }

           // Phone     [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]


        [DisplayName("Phone")]
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"\(?\d{3}\)?[-\s.]?\d{3}[-.]\d{4}( x\d{0,5})?",
            ErrorMessage = "Phone is not in a valid format.")]      
        public string Phone { get; set; }


        [DisplayName("Email")]
        [Required(ErrorMessage = "Please enter your email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^[_a-zA-Z0-9-\']+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))$",
            ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }


        [UIHint("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^.*(?=.{8,30}$)(?=.*\d)(?=.*[A-Z])(?=.*[\&quot;!#$%'()*+,-./:;=>?@[\]^_`{|}~]).*$",
            ErrorMessage = "All passwords must now be between 8 to 30 characters and must contain at least one upper case letter, at least one digit and at least one special character, with the exception of the characters < and &, which cannot be used.")]
        public string Password { get; set; }

        [UIHint("Password")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [Compare("Password", ErrorMessage = "Your passwords do not match.")]
        public string ConfirmPassword { get; set; }


        [DisplayName("AddToDistributionList")]
        public bool AddToDistributionList { get; set; }

    }

    /// <summary>
    /// Login View Model
    /// </summary>
    public class LoginViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^\s*[_a-zA-Z0-9-\']+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))\s*$",
            ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [UIHint("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }

        //[HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }
    }

    //Forgotten Password View Model
    public class ForgottenPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [RegularExpression(@"^[_a-zA-Z0-9-\']+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))$",
            ErrorMessage = "Please enter a valid email address")]

        public string EmailAddress { get; set; }
    }


    //Reset Password View Model
    public class ResetPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [UIHint("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^.*(?=.{8,30})(?=.*\d)(?=.*[A-Z])(?=.*[\&quot;!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]).*$",
            ErrorMessage = "All passwords must now be between 8 to 30 characters and must contain at least one upper case letter, at least one digit and at least one special character.")]
        public string Password { get; set; }

        [UIHint("Password")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^.*(?=.{8,30})(?=.*\d)(?=.*[A-Z])(?=.*[\&quot;!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]).*$",
            ErrorMessage = "All passwords must now be between 8 to 30 characters and must contain at least one upper case letter, at least one digit and at least one special character.")]
        [Compare("Password", ErrorMessage = "Your passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }



    //Approval  View Model
    public class UserApprovalViewModel
    {
        public int ID { get; set; }

        public bool IsUserApproved { get; set; }

        public bool IsUserInactive { get; set; }

        public bool IsUserVerifiedEmail { get; set; }

        public string SubmitAction { get; set; }


        public string Name { get; set; }



        [DisplayName("Role Applied")]
        public string RoleApplied { get; set; }



        [DisplayName("Role Assigned")]
        public string RoleAssigned { get; set; }

        [DisplayName("Role Name")]
        public string Role { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State name is required")]
        public string State { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "City name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string City { get; set; }

        [DisplayName("Organization")]
        [Required(ErrorMessage = "Organization  name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string Organization { get; set; }

        [DisplayName("Reason")]
        [Required(ErrorMessage = "Reason  for this request  is required")]
        [RegularExpression(@"^[a-zA-Z0-9\.,\n'\-.\s]{1,200}$",
            ErrorMessage = "Only alphabets, numbers, apostrophe, hyphens, comma and period allowed; maximum 200 characters.")]
        public string Reason { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z'\-.\s]{1,100}$",
            ErrorMessage = "Only alphabets, apostrophe, hyphens allowed; maximum 100 characters.")]
        public string LastName { get; set; }


        [DisplayName("Primary Email")]
        public string EmailAddress { get; set; }

        [DisplayName("Primary Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^[_a-zA-Z0-9-\']+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))$",
            ErrorMessage = "Please enter a valid email address")]
        public string EmailAddressChange { get; set; }

        [DisplayName("Phone")]
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }


        [DisplayName("AddToDistributionList")]
        public  bool AddToDistributionList { get; set; }

    }

}