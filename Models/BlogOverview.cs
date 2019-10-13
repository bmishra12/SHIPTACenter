using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Cultiv.Models
{
    public class BlogOverview : RenderModel
    {
        public BlogOverview() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent) { }
        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int PreviousPage { get; set; }

        public int NextPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public IEnumerable<IPublishedContent> BlogPosts { get; set; }


        //sammit:lesson
        //whether the tempdata[success] true or false  @if (success) is true
        //as it checks for not null.. so only if you want to send  
        //@if (success) == true you set tempdata[success] = true
        //
    }
}