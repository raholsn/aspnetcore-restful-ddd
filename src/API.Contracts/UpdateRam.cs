using System;

namespace API.Contracts
{
    public class UpdateRam
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
