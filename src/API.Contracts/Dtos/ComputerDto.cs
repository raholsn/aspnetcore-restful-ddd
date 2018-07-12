using System;
using System.Collections.Generic;

namespace API.Contracts.Dtos
{
    public class ComputerDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public IEnumerable<RamDto> Rams { get; set; }
        public IEnumerable<GraphicsCardDto> GraphicCards { get; set; }
    }
}