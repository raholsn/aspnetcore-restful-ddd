using System;
using System.Collections.Generic;
using System.Text;

namespace API.Contracts
{
    public class CreateRam
    {
        public Guid ComputerId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
