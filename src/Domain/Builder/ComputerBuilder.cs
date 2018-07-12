using System;
using System.Collections.Generic;

namespace Domain.Builder
{
    public class ComputerBuilder
    {
        private Computer _computer;
        public ComputerBuilder CreateComputer(string name, double price)
        {
            _computer = new Computer(name, price);
            return this;
        }

        public ComputerBuilder WithGraphicsCards(IEnumerable<GraphicsCard> graphicsCards)
        {
            _computer.GraphicCards = (ICollection<GraphicsCard>)graphicsCards;
            return this;
        }
        public ComputerBuilder WithRams(IEnumerable<Ram> rams)
        {
            this._computer.Rams = (ICollection<Ram>)rams;
            return this;
        }

        public Computer Build()
        {
            if (_computer == null)
            {
                throw new Exception("Could not create computer");
            }

            if (_computer.GraphicCards != null)
            {
                foreach (var graphicsCard in _computer.GraphicCards)
                {
                    graphicsCard.ComputerId = _computer.Id;
                }
            }

            if (_computer.Rams != null)
            {
                foreach (var ram in _computer.Rams)
                {
                    ram.ComputerId = _computer.Id;
                }
            }

            return _computer;
        }

    }
}
