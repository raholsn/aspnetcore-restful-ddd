using System;

namespace Domain.Builder
{
    public class RamBuilder
    {
        private Ram ram;

        public RamBuilder CreateRam(Guid id, double price, string name, Guid computerId)
        {
            ram = new Ram(id, price, name, computerId);
            return this;
        }

        public Ram Build()
        {
            return this.ram;
        }
    }
}
