using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Example.Models
{
    public class Person
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }

        [IgnoreDataMember]
        public ICollection<int> Friends { get; set; }
    }
}
