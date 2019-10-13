
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;



namespace UmbracoShipTac.Models
{


    public class ResourceTopViewModel
    {
        public IEnumerable<SResult> TopListView { get; set; }

    }

    public class UserFeatureTopViewModel
    {
        public int PendingCount { get; set; }
        public IEnumerable<SResult> TopListView { get; set; }

    }

    public class Activities
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public bool Checked { get; set; }
    }


    public class ResourceView
    {
        public string ID { get; set; }

        public string Name { get; set; }
        public string Contributor { get; set; }
        public string CreatedDate { get; set; }

        public string Status { get; set; }

        public string State { get; set; }
    }


    /// <summary>
    /// user list View Model
    /// </summary>
    public class PendingResourceViewModel
    {

        [DisplayName("State")]
        public string State { get; set; }


        [DisplayName("Topic")]
        public string Topic { get; set; }


        public IEnumerable<ResourceView> ResourceView { get; set; }


        public IList<Activities> UiSubject { get; set; }
        public IList<Activities> UiActivity { get; set; }
        public IList<Activities> UiAudience { get; set; }

        public IList<Activities> UiFileTypes { get; set; }

        //this will retrieve selected in list when submitted
        public List<string> SubmittedActivities { get; set; }

        public IEnumerable<string> SelectedSubjects { get; set; }
        public IEnumerable<string> SelectedActivities { get; set; }
        public IEnumerable<string> SelectedAudences { get; set; }
        public IEnumerable<string> SelectedFileTypes { get; set; }

    }


     public class SearchResourceViewModel
    {

        [DisplayName("State")]
        public string State { get; set; }


        [DisplayName("Topic")]
        public string Topic { get; set; }


        public string ResTerm { get; set; }


        public bool ExactMatch { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
 
        public IEnumerable<SResult> ResourceView { get; set; }


        public IList<Activities> UiSubject { get; set; }
        public IList<Activities> UiActivity { get; set; }
        public IList<Activities> UiAudience { get; set; }

        public IList<Activities> UiFileTypes { get; set; }

        //this will retrieve selected in list when submitted
        public List<string> SubmittedActivities { get; set; }

        public IEnumerable<string> SelectedSubjects { get; set; }
        public IEnumerable<string> SelectedActivities { get; set; }
        public IEnumerable<string> SelectedAudences { get; set; }
        public IEnumerable<string> SelectedFileTypes { get; set; }

    }


    public class ResourceViewModel
    {

        public int ResourceId { get; set; }

        // Title (enter the title that should appear in the library for your resource): [allow for up to 100 characters]
        [Required(ErrorMessage = "Title is required")]

        [RegularExpression(@"^[^<>.!^@#%/]{1,100}$",
    ErrorMessage = "Special characters not allowed :^<>.!@#%/; maximum 100 characters.")]
        public string Title { get; set; }

        //Subject (select all that apply): [users will be provided with a drop down list]:
        public string Subject { get; set; }

        //Activity (select all that apply): [users will be provided with a drop down list]:
        public string Activity { get; set; }

        public string DateRange { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please remove all protected health information or other sensitive identifying information from your resource and re-upload the files or links.")]
        public bool IsVerifyCheked { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please remove all protected health information or other sensitive identifying information from your resource and re-upload the files or links.")]
        public bool IsCautionCheked { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }


        //[HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        public string Role { get; set; }

        public string State { get; set; }

        //[RegularExpression("True", ErrorMessage = "You must accept the terms and conditions")]

        public IList<Activities> UiSubject { get; set; }
        public IList<Activities> UiActivity { get; set; }
        public IList<Activities> UiAudience { get; set; }

        public IList<Activities> UiFileTypes { get; set; }


        [DisplayName("Select File to Upload")]
        public IEnumerable<HttpPostedFileBase> File { get; set; }



        public string myCaption1 { get; set; }
        public string myLink1 { get; set; }

        public string myCaption2 { get; set; }
        public string myLink2 { get; set; }

        public string Contributor { get; set; }
        public string ContributorEmail { get; set; }


    }

    public class RLinks
    {
        public string caption { get; set; }
        public string link { get; set; }
        public bool newWindow { get; set; }
        public bool edit { get; set; }
        public bool isInternal { get; set; }
        public string type { get; set; }
        public string title { get; set; }

    }
    public class ResourceApproveViewModel
    {

        public int ResourceId { get; set; }

        // Title (enter the title that should appear in the library for your resource): [allow for up to 100 characters]
        [Required(ErrorMessage = "Title is required")]

        [RegularExpression(@"^[^<>.!^@#%/]{1,100}$",
    ErrorMessage = "Special characters not allowed :^<>.!@#%/; maximum 100 characters.")]
        public string Title { get; set; }

        //Subject (select all that apply): [users will be provided with a drop down list]:
        public string Subject { get; set; }

        //Activity (select all that apply): [users will be provided with a drop down list]:
        public string Activity { get; set; }


        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }


        //[HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        public string Role { get; set; }

        public string State { get; set; }

        //[RegularExpression("True", ErrorMessage = "You must accept the terms and conditions")]

        public IList<Activities> UiSubject { get; set; }
        public IList<Activities> UiActivity { get; set; }
        public IList<Activities> UiAudience { get; set; }

        public IList<Activities> UiFileTypes { get; set; }

        public IList<UrlLink> MediaUrls { get; set; }

        public IList<RLinks> MyRlinks { get; set; }



        //Intended Audience (select all of the appropriate audiences for your resource, using the drop down list provided):
        public string IntendedAudience { get; set; }


        public string SubmitAction { get; set; }
        public string Contributor { get; set; }
        public string ContributorEmail { get; set; }



    }
    public class UrlLink
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

}