using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Example.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            // This represents the root of your api, that will contain the links
            // to all other resources.
            return Ok(new Resource<object>
            {
                Links = new List<ILink>
                {
                    new Link("people", Url.Action("Get", "People"))
                },
                SingularRelations = new HashSet<string>
                {
                    "self",
                    "people"
                }
            });
        }
    }
}
