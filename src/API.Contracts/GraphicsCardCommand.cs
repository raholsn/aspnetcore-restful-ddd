using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API.Contracts
{
    public abstract class GraphicsCardCommand 
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
