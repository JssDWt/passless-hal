using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Example.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        static int maxId = 1;
        static readonly ICollection<Person> People = new List<Person>
        {
            new Person
            {
                Id = 1,
                Name = "Jesse de Wit",
                DateOfBirth = new DateTime(1988, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                Address = "*secret*"
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            return Ok(People);
        }

        [HttpGet("{id}")]
        public ActionResult<Person> Detail(int id)
        {
            var person = People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [HttpPost]
        public ActionResult<Person> Post([FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            person.Id = Interlocked.Increment(ref maxId);
            People.Add(person);
            return Created(Url.Action(nameof(Get), new { person.Id }), person);
        }
    }
}
