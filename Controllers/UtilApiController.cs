using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.WebApi;

namespace UmbracoShipTac.Controllers
{
    public class UtilApiController : UmbracoApiController
    {

        public IEnumerable GetAllValues()
        {
            return new[] { "value 1", "value 2", "value 3", "value 4" };
        }
    }
}