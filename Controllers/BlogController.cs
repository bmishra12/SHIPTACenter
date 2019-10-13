//CONTROLLER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cultiv.Models;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace Cultiv.Controllers
{
    public class BlogOverviewController : RenderMvcController
    {
        public ActionResult BlogOverview(BlogOverview model)
        {
            model.BlogPosts = GetPagedBlogPosts(model);

            return CurrentTemplate(model);
        }

        private static IEnumerable<IPublishedContent> GetPagedBlogPosts(BlogOverview model)
        {
            if (model.Page == default(int))
                model.Page = 1;

            const int pageSize = 5;
            var skipItems = (pageSize * model.Page) - pageSize;

            var posts = model.Content.Children.ToList();
            model.TotalPages = Convert.ToInt32(Math.Ceiling((double)posts.Count() / pageSize));

            model.PreviousPage = model.Page - 1;
            model.NextPage = model.Page + 1;

            model.IsFirstPage = model.Page <= 1;
            model.IsLastPage = model.Page >= model.TotalPages;

            return posts.OrderByDescending(x => x.CreateDate).Skip(skipItems).Take(pageSize);
        }
    }
}