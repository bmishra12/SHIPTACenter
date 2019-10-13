using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbracoShipTac.Models
{
    /// <summary>
    /// user list View Model
    /// </summary>
    public class PendingUserViewModel
    {

        [DisplayName("State")]
        public string State { get; set; }


        [DisplayName("NameSearch")]
        public string NameSearch { get; set; }

        public IEnumerable<UserView> UsersView { get; set; }

    }


    public class UserViewModel
    {

        [DisplayName("State")]
        public string State { get; set; }

        [DisplayName("NameSearch")]
        public string NameSearch { get; set; }

        public IEnumerable<UserView> UsersView { get; set; }

    }


    public class SearchViewModel
    {

        [DisplayName("term")]
        public string Term { get; set; }

        public IEnumerable<SResult> SerchsView { get; set; }

        public IEnumerable<SResult> SerchResourcView { get; set; }

        public IEnumerable<SResult> SerchEventView { get; set; }


    }



    public class ProductTyreSelectorViewModel : RenderModel
    {
        //public ProductTyreSelectorViewModel(RenderModel model)
        //    : base(model.Content, model.CurrentCulture)
        //{
        //    Tyres = new List<UserView>();
        //}


        public ProductTyreSelectorViewModel() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent) { }

        public ProductTyreSelectorViewModel(IPublishedContent content, CultureInfo culture)
            : base(content, culture)
        {
        }

        public ProductTyreSelectorViewModel(IPublishedContent content)
            : base(content)
        {
        }

      ////  public IList<UserView> Tyres { get; set; }

        public IEnumerable<UserView> UsersView { get; set; }
    }


    public class SResult
    {

        public string Id { get; set; }

        public string Url { get; set; }
        public string NodeName { get; set; }
        public int Score { get; set; }
        public string Text { get; set; }
        public string FText { get; set; }

        public string ImageUrl { get; set; }


        public string Date { get; set; }
        public int Order { get; set; }

    }
    public class UserView
    {
        public string ID { get; set; }

        public string Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedDate { get; set; }
        public string LastLoginDate { get; set; }

        public string ApprovedDate { get; set; }
        public string DeniedDate { get; set; }

        public string Status { get; set; }

        public string State { get; set; }
    }



}