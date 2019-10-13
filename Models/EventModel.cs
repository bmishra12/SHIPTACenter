
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace UmbracoShipTac.Models
{

    public class EventViewModel
    {

        public int EventId { get; set; }

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

        public string IntendedAudience { get; set; }


        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }


        //[HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        public string Role { get; set; }

        public string State { get; set; }

        //[RegularExpression("True", ErrorMessage = "You must accept the terms and conditions")]

        public string myCaption1 { get; set; }
        public string myLink1 { get; set; }

        public string myCaption2 { get; set; }
        public string myLink2 { get; set; }

        public string Contributor { get; set; }
        public string ContributorEmail { get; set; }


    }


    public class WeekForMonth
    {
        public List<Day> Week1 { get; set; }
        public List<Day> Week2 { get; set; }
        public List<Day> Week3 { get; set; }
        public List<Day> Week4 { get; set; }
        public List<Day> Week5 { get; set; }
        public List<Day> Week6 { get; set; }
        public string nextMonth { get; set; }
        public string prevMonth { get; set; }


        public int intMonth { get; set; }
        public int intYear { get; set; }

        public string curMonth { get; set; }
        public string year { get; set; }

    }

    public class Day
    {
        public DateTime Date { get; set; }
        public string _Date { get; set; }
        public string dateStr { get; set; }
        public int dtDay { get; set; }
        public int? daycolumn { get; set; }

        public List<ShipEvent> ShipEvents { get; set; }

    }
    public class ShipEvent
    {

        public string eventDesc { get; set; }
        public string eventTime { get; set; }

        public string eventLink { get; set; }
    }

    public class PopulateMonth
    {
        public int month { get; set; }
        public int totaldayformonth { get; set; }
        public int year { get; set; }
    }


    public class EventResult
    {

        public string Id { get; set; }

        public string Url { get; set; }
        public string NodeName { get; set; }
        public int Score { get; set; }
        public string Text { get; set; }
        public string FText { get; set; }

        public string sdtTime { get; set; }
        public string sdtTimeTo { get; set; }

        public int day { get; set; }

        public int month { get; set; }
        public int year { get; set; }

        public string timeFrom { get; set; }
      public string timeTo { get; set; }
        


    }
}