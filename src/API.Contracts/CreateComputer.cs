using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API.Contracts
{
    public class CreateComputer
    {
        [Required]
        [MaxLength(255),]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }
        public IEnumerable<CreateGraphicsCard> GraphicsCard { get; set; }
        public IEnumerable<CreateRam> Ram { get; set; }
    }
}
