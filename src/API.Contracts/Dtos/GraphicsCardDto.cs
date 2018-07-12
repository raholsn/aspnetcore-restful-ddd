using System;

namespace API.Contracts.Dtos
{
    public class GraphicsCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public Guid ComputerId { get; set; }
    }
}