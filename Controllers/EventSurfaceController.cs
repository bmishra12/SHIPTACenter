using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using UmbracoShipTac.Models;
using UmbracoShipTac.Code;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Examine;
using Examine.SearchCriteria;


namespace UmbracoShipTac.Controllers.SurfaceControllers
{


    public class EventSurfaceController : SurfaceController
    {


        public ActionResult RenderAddEvent()
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                return PartialView("authsurface/notauth");
            }

            if (!System.Web.HttpContext.Current.User.IsInRole("shipcenter"))
                return PartialView("authsurface/notauth");
            //redirect can not happen in Render..
            //return Redirect("/notauthorized");

            EventViewModel model = new EventViewModel();

            return PartialView("eventsurface/eventcreate", model);
        }



        [HttpPost]
        public ActionResult HandleAddEvent(EventViewModel model)
        {
            bool isShipAdmin = false;
            bool isStateAdmin = false;
            string fname = string.Empty;
            string lname = string.Empty;
            string userName = string.Empty;
            string userEmail = System.Web.HttpContext.Current.User.Identity.Name;

            if (!ModelState.IsValid)
            {
                //return to current page 
                return CurrentUmbracoPage();
            }

            // create the RLinks list
            var relatedLinks = new List<RLinks>();

            if (System.Web.HttpContext.Current.User.IsInRole("shipcenter")) isShipAdmin = true;

            if (System.Web.HttpContext.Current.User.IsInRole("shipdirector") || System.Web.HttpContext.Current.User.IsInRole("acladmin") || System.Web.HttpContext.Current.User.IsInRole("shipadmin")) isStateAdmin = true;

            //extract the from date to date and timefrom and time to from below format
            //06/26/2016 12:00 AM - 06/26/2016 11:59 PM
            string[] dates = model.DateRange.Split('-');

            //from date
            DateTime fromDate;
            if (!DateTime.TryParse(dates[0], out fromDate))
            {
                // handle parse failure
            }

            //todate
            DateTime toDate;
            if (!DateTime.TryParse(dates[1], out toDate))
            {
                // handle parse failure
            }
             


            //create the content..

            //chose the correct EventNode
            //if the user role is shipCenter then pick up the intended role chosen.
            //else it is unpublished node..
            int parentID = 0;
            string state = "";
            string roles = "";


            //check which role is selected and determine the parent eventsid
            if (model.Role == "1")
            {
                parentID = ConfigUtil.EventE1Id;
                roles = "shipcenter,acladmin,partner,shipdirector,shipadmin,shipstaff,shipcounselor";
            }
            else if (model.Role == "2")
            {
                parentID = ConfigUtil.EventE2Id;
                roles = "shipcenter,acladmin,partner,shipdirector,shipadmin,shipstaff";
            }
            else if (model.Role == "3")
            {
                parentID = ConfigUtil.EventE3Id;
                roles = "shipcenter,acladmin,partner,shipdirector,shipadmin";
            }
            else if (model.Role == "4")
            {
                parentID = ConfigUtil.EventE4Id;
                   roles = "shipcenter,acladmin,partner,shipdirector";
            }
            else //This situation will never occur but just in case default - visible to larger audience
            {
                parentID = ConfigUtil.EventE1Id;
                roles = "shipcenter,acladmin,partner,shipdirector,shipadmin,shipstaff,shipcounselor";
            }


            var userService = ApplicationContext.Current.Services.MemberService;
            var amember = userService.GetByEmail(userEmail);
            string astate = amember.GetValue("state").ToString();
            fname = amember.GetValue("firstName").ToString();
            lname = amember.GetValue("lastName").ToString();
            userName = string.Format("{0} {1}", fname, lname);



            int userID = 0;

            Umbraco.Core.Services.ContentService csv = new Umbraco.Core.Services.ContentService();


            var con = csv.CreateContent(model.Title, parentID, "Event", userID);

            //set all the values
            con.SetValue("eventtitle", model.Title);
            con.SetValue("description", model.Description);

            con.SetValue("registerlink", model.myLink1);
            con.SetValue("registercaption", model.myCaption1);

            //con.SetValue("state", state);

            con.SetValue("contributor", userName);
            con.SetValue("contributoremail", userEmail);

            //dates
            con.SetValue("eventdatetime", model.DateRange);
           
            con.SetValue("datetimefrom", fromDate.ToString());
            con.SetValue("datetimeto", toDate.ToString());
            
            con.SetValue("eventmonth", fromDate.Month.ToString());
            con.SetValue("eventyear", fromDate.Year.ToString());

            con.SetValue("intendedaudience", model.IntendedAudience);

    
                
            //populate the roles too.
            con.SetValue("roles", roles);

            csv.SaveAndPublishWithStatus(con);

            //Return the view...
            TempData["eID"] = con.Id;
            TempData["success"] = true;
            return CurrentUmbracoPage();



        }



        private void AddGroupedCriteria(ref IBooleanOperation filter, string field, string value)
        {
            string[] values = value.Split(',');
            var fields = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                fields[i] = field;
            }
            filter.And().GroupedOr(fields, values);

        }


        public ActionResult RenderEventCalander(WeekForMonth model)
        {
            bool hasloggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!hasloggedin)
            {
                //this does not work.. you can not redirect from Render
                //We can not show "NOAUTH"
                //we can show a login message.. 

                return PartialView("authsurface/loginagain");
                //return Redirect("/");
            }

            //Get the querystring 
            var action = Request.QueryString["SubmitAction"];
             int month = System.DateTime.Today.Month;
             int year = System.DateTime.Today.Year;

            //Ensure we have a vlaue in QS
            if (string.IsNullOrEmpty(action))
            {
                //populate the calender -all the weeks 
                //and assign to the model..

                model = getCalender(month, year);
                model.intMonth = month; //assign the month
                model.intYear = year;
            }
            else if (action == "Next")
            {
                int _mo = model.intMonth + 1;
                int _year = model.intYear;
 
                if (model.intMonth == 12)
                {
                    _mo = 1;
                    _year++;
                }

                model = getCalender(_mo, _year);
    
                model.intMonth = _mo; //assign the next month
                model.intYear = _year;


            }
            else if (action == "Prev")
            {

                int _mo = model.intMonth - 1;
                int _year = model.intYear;
                if (model.intMonth == 1)
                {
                    _mo = 12;
                    _year--;
                }



                model = getCalender(_mo, _year);
    
                model.intMonth = _mo; //assign the prev month
                model.intYear = _year; 
            }


            //now add all the role basedd events to the populated calander

            return PartialView("eventsurface/eventcalendar", model);

        }


        //TODO: add correct search criteria 
        //year  month and roleBased 
        private List<EventResult> SearchEvents(int month, int year)
        {

            //the resource will be searched only for the logged in users

            List<EventResult> results = new List<EventResult>();

            // Find pages that contain our search text in either their nodeName or bodyText fields...
            // but exclude any pages that have been hidden.
            // searchCriteria.Fields("nodeName",terms.Boost(8)).Or().Field("metaTitle","hello".Boost(5)).Compile();

            //if nothing selected return null result
            //if (model.ResTerm.IsNullOrWhiteSpace()
            //    && model.SelectedFileTypes == null
            //    && model.SelectedSubjects == null
            //    && model.SelectedAudences == null
            //    && model.SelectedActivities == null
            //    && model.State.IsNullOrWhiteSpace())
            //{
            //    return results;
            //}

            if (User.Identity.IsAuthenticated)
            {
                string[] roles = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name);

                //Event search

                var criteriaEventRestricted = ExamineManager.Instance
                 .SearchProviderCollection["EventSearcher"]
                 .CreateSearchCriteria(BooleanOperation.Or);

                Examine.SearchCriteria.IBooleanOperation filter = null;


                filter = criteriaEventRestricted.Field("IsPublic", "false"); //just to start a dummy this is OR

                filter = filter.And().Field("roles", roles[0]);
                filter = filter.And().Field("eventmonth", month.ToString());
                filter = filter.And().Field("eventyear", year.ToString());

                //add date and month
                //filter = filter.And().Field()

                //if (model.ResTerm != null && model.ResTerm.Length > 0)
                //    filter = filter.And().GroupedOr(new string[] { "description", "eventtitle", "nodeName", "name", "seo", "intendedaudiences"}, model.ResTerm);



                ISearchResults SearchResults = ExamineManager.Instance
                    .SearchProviderCollection["EventSearcher"]
                    .Search(filter.Compile());


                foreach (var sr in SearchResults)
                {
                    EventResult result = new EventResult()
                    {
                        Id = sr.Fields.ContainsKey("id") ? sr.Fields["id"] : "",
                        Url = Umbraco.Content(sr.Fields["id"]).Url,
                        NodeName = sr.Fields.ContainsKey("nodeName") ? sr.Fields["nodeName"] : "",
                        Text = sr.Fields.ContainsKey("description") ? sr.Fields["description"] : "",
                        sdtTime = sr.Fields.ContainsKey("datetimefrom") ? sr.Fields["datetimefrom"] : "",
                        sdtTimeTo = sr.Fields.ContainsKey("datetimeto") ? sr.Fields["datetimeto"] : "", 
  

                    };

                    result.Text = result.Text.Substring(0, Math.Min(result.Text.Length, 250));

                   
                    DateTime myDate;
                    if (DateTime.TryParse(result.sdtTime, out myDate))
                    {
                        result.day = myDate.Day;
                        result.month = myDate.Month;
                        result.year = myDate.Year;
                        result.timeFrom = myDate.ToShortTimeString();
                       
                        
                    }
                    else
                    {
                        // handle parse failure
                    }

                   DateTime myDateTo;
                    if (DateTime.TryParse(result.sdtTimeTo, out myDateTo))
                    {

                        result.timeTo = myDateTo.ToShortTimeString();
                       
                      
                    }
                    else
                    {
                        // handle parse failure
                    }


                    
                    results.Add(result);
                }

            }


            return results;


        }




        public WeekForMonth getCalender(int month, int year)
        {

            WeekForMonth weeks = new WeekForMonth();

            //set the month and yer..
            weeks.curMonth = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            weeks.year = year.ToString();

            weeks.Week1 = new List<Day>();
            weeks.Week2 = new List<Day>();
            weeks.Week3 = new List<Day>();
            weeks.Week4 = new List<Day>();
            weeks.Week5 = new List<Day>();
            weeks.Week6 = new List<Day>();

            List<DateTime> dt = new List<DateTime>();
            dt = GetDates(year, month);


            //get the events
            List<EventResult> events = SearchEvents(month, year);


            //DateTime[] events = new DateTime[]
            //    {
            //        new DateTime(DateTime.Now.Year, 7, 1), 
            //        new DateTime(DateTime.Now.Year, 7, 2), 
            //        new DateTime(DateTime.Now.Year, 7, 3)
            //    };


             foreach (DateTime day in dt)
            {

                //check if this date exist in the event List..
               // bool exist = events.Any(d => d.Month == day.Month && d.Day == day.Day);

                 //get the particular event/s on that day.
                // IEnumerable<DateTime> evs = events.Where(d => d.Month == day.Month && d.Day == day.Day);


                IEnumerable<EventResult> evs = events.Where(d => d.month == day.Month && d.year == day.Year && d.day == day.Day).OrderBy(d => d.timeFrom); 


                switch (GetWeekOfMonth(day))
                {
                    case 1:
                        Day dy1 = new Day();

                        dy1.Date = day;
                        dy1._Date = day.ToShortDateString();
                        dy1.dateStr = day.ToString("MM/dd/yyyy");
                        dy1.dtDay = day.Day;
                        dy1.daycolumn = GetDateInfo(dy1.Date);

                        dy1.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy1.ShipEvents.Add(
                                new ShipEvent 
                                { eventDesc= e.NodeName, 
                                  eventLink= e.Url,
                                  eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }

                        weeks.Week1.Add(dy1);
                        break;
                    case 2:
                        Day dy2 = new Day();
                        dy2.Date = day;
                        dy2._Date = day.ToShortDateString();
                        dy2.dateStr = day.ToString("MM/dd/yyyy");
                        dy2.dtDay = day.Day;
                        dy2.daycolumn = GetDateInfo(dy2.Date);

                        dy2.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy2.ShipEvents.Add(
                                new ShipEvent
                                {
                                    eventDesc = e.NodeName,
                                    eventLink = e.Url,
                                    eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }

                        weeks.Week2.Add(dy2);
                        break;
                    case 3:
                        Day dy3 = new Day();
                        dy3.Date = day;
                        dy3._Date = day.ToShortDateString();
                        dy3.dateStr = day.ToString("MM/dd/yyyy");
                        dy3.dtDay = day.Day;
                        dy3.daycolumn = GetDateInfo(dy3.Date);

                                                dy3.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy3.ShipEvents.Add(
                                new ShipEvent
                                {
                                    eventDesc = e.NodeName,
                                    eventLink = e.Url,
                                    eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }


                        weeks.Week3.Add(dy3);
                        break;
                    case 4:
                        Day dy4 = new Day();
                        dy4.Date = day;
                        dy4._Date = day.ToShortDateString();
                        dy4.dateStr = day.ToString("MM/dd/yyyy");
                        dy4.dtDay = day.Day;
                        dy4.daycolumn = GetDateInfo(dy4.Date);

                                                dy4.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy4.ShipEvents.Add(
                                new ShipEvent
                                {
                                    eventDesc = e.NodeName,
                                    eventLink = e.Url,
                                    eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }


                        weeks.Week4.Add(dy4);
                        break;
                    case 5:
                        Day dy5 = new Day();
                        dy5.Date = day;
                        dy5._Date = day.ToShortDateString();
                        dy5.dateStr = day.ToString("MM/dd/yyyy");
                        dy5.dtDay = day.Day;
                        dy5.daycolumn = GetDateInfo(dy5.Date);

                                                dy5.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy5.ShipEvents.Add(
                                new ShipEvent
                                {
                                    eventDesc = e.NodeName,
                                    eventLink = e.Url,
                                    eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }

                        weeks.Week5.Add(dy5);
                        break;
                    case 6:
                        Day dy6 = new Day();
                        dy6.Date = day;
                        dy6._Date = day.ToShortDateString();
                        dy6.dateStr = day.ToString("MM/dd/yyyy");
                        dy6.dtDay = day.Day;
                        dy6.daycolumn = GetDateInfo(dy6.Date);

                                                dy6.ShipEvents = new List<ShipEvent>();

                        //add the events if any.. max 2 for now
                        foreach (EventResult e in evs.Take(2))
                        {

                            dy6.ShipEvents.Add(
                                new ShipEvent
                                {
                                    eventDesc = e.NodeName,
                                    eventLink = e.Url,
                                    eventTime = e.timeFrom + " - " + e.timeTo
                                });

                        }

                        weeks.Week6.Add(dy6);
                        break;
                };
            }

            while (weeks.Week1.Count < 7) // not starting from sunday
            {
                Day dy = null;
                weeks.Week1.Insert(0, dy);
            }

            if (month == 12)
            {
                weeks.nextMonth = (01).ToString() + "/" + (year + 1).ToString();
                weeks.prevMonth = (month - 1).ToString() + "/" + (year).ToString();
            }
            else if (month == 1)
            {
                weeks.nextMonth = (month + 1).ToString() + "/" + (year).ToString();
                weeks.prevMonth = (12).ToString() + "/" + (year - 1).ToString();
            }
            else
            {
                weeks.nextMonth = (month + 1).ToString() + "/" + (year).ToString();
                weeks.prevMonth = (month - 1).ToString() + "/" + (year).ToString();
            }

            return weeks;
        }

        public static List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList();
        }

        /*   public static int GetWeekNumber(DateTime dtPassed)
           {
               CultureInfo ciCurr = CultureInfo.CurrentCulture;

               int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
               return weekNum;
           }*/

        public static int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            while (date.Date.AddDays(1).DayOfWeek != DayOfWeek.Sunday)//CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);
            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        public int GetDateInfo(DateTime now)
        {
            int dayNumber = 0;
            DateTime dt = now.Date;
            string dayStr = Convert.ToString(dt.DayOfWeek);

            if (dayStr.ToLower() == "sunday")
            {
                dayNumber = 0;
            }
            else if (dayStr.ToLower() == "monday")
            {
                dayNumber = 1;
            }
            else if (dayStr.ToLower() == "tuesday")
            {
                dayNumber = 2;
            }
            else if (dayStr.ToLower() == "wednesday")
            {
                dayNumber = 3;
            }
            else if (dayStr.ToLower() == "thursday")
            {
                dayNumber = 4;
            }
            else if (dayStr.ToLower() == "friday")
            {
                dayNumber = 5;
            }
            else if (dayStr.ToLower() == "saturday")
            {
                dayNumber = 6;
            }
            return dayNumber;
        }


    }
}
