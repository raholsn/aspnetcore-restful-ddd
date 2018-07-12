using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ram
    {
        [Key]
        public Guid Id { get; set; }    

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public Guid ComputerId { get; set; }

        internal Ram(Guid id, double price, string name, Guid computerId)
        {
            ComputerId = computerId;
            Price = price;
            Name = name;
            Id = id;
        }

        public Ram() { }

        public void Update(string name, double price)
        {
            this.Price = price;
            this.Name = name;
        }
    }
}