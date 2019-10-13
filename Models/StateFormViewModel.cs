using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UmbracoShipTac.Models
{
    public class StateFormViewModel
    {
    
        public int NodeIdToRedirect { get; set; }

        [Required]
        public int PageId { get; set; }

        // Properties
        public IEnumerable<SelectListItem> ListName { get; set; }

    }
}