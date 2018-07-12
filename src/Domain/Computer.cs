using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Computer
    {
        public Computer()
        {
            Rams = new List<Ram>();
            GraphicCards = new List<GraphicsCard>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public ICollection<Ram> Rams { get; set; }

        public ICollection<GraphicsCard> GraphicCards { get; set; }

        internal Computer(string name, double price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }

        public void Update(string name, double price)
        {
            Name = name;
            Price = price;
        }
    }
}