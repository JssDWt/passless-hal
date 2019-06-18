using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Example.Models;
using Microsoft.AspNetCore.Mvc;
using Passless.Hal;

namespace Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [HalStaticLink("google", "https://www.google.com/", IsSingular = true)]
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
                Address = "*secret*",
                Friends = new int[]
                {
                    3,
                    4
                }
            },
            new Person
            {
                Id = 2,
                Name = "Sjaak de Wit",
                DateOfBirth = new DateTime(1950, 11, 7, 0, 0, 0, DateTimeKind.Utc),
                Address = "*secret*"
            },
            new Person
            {
                Id = 3,
                Name = "Cass Gooskens",
                DateOfBirth = new DateTime(1989, 6, 28, 0, 0, 0, DateTimeKind.Utc),
                Address = "*secret*",
                Friends = new int[]
                {
                    1
                }
            },
            new Person
            {
                Id = 4,
                Name = "Jelle Boterman",
                DateOfBirth = new DateTime(1988, 3, 7, 0, 0, 0, DateTimeKind.Utc),
                Address = "*secret*",
                Friends = new int[]
                {
                    1
                }
            }
        };

        [HttpGet]
        [HalLinkAction("self", nameof(Get))]
        public ActionResult<IEnumerable<Person>> Get()
        {
            // Note that this method returns Person objects, not IResource objects.
            // These are converted into HAL resources in the ResourceFactory (see Startup.cs)
            return Ok(People);
        }

        [HttpGet("{id}")]
        [HalEmbedAction("friends", nameof(Friends))]
        [HalLinkAction("friends", nameof(Friends), Parameter = "Id")]
        [HalLinkAction("self", nameof(Detail), Parameter = "Id")]
        public ActionResult<Person> Detail(int id)
        {
            var person = People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            // Note that this method returns a Person object, not an IResource object.
            // This is converted into a HAL resource in the ResourceFactory (see Startup.cs)
            return Ok(person);
        }

        [HttpGet("{id}/friends")]
        public ActionResult<IEnumerable<Person>> Friends(int id)
        {
            var person = People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            var friends = People.Where(p => person.Friends?.Contains(p.Id) ?? false).ToList();
            return Ok(friends);
        }

        [HttpPost]
        [HalLinkAction("self", nameof(Detail), Parameter = "Id")]
        public ActionResult<Person> Post([FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            person.Id = Interlocked.Increment(ref maxId);
            People.Add(person);

            // Note that this method returns a Person object, not an IResource object.
            // This is converted into a HAL resource in the ResourceFactory (see Startup.cs)
            return Created(Url.Action(nameof(Get), new { person.Id }), person);
        }
    }
}
