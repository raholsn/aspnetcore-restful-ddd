using System;

namespace Domain.Builder
{
    public class GraphicsCardBuilder : IGraphicsCardBuilder
    {
        private GraphicsCard graphicsCard;

        public GraphicsCardBuilder CreateGraphicsCard(double price, string name, Guid computerId)
        {
            graphicsCard = new GraphicsCard(new Guid(), price, name, computerId);
            return this;
        }

        public GraphicsCardBuilder UpdateGraphicsCard(Guid id, double price, string name, Guid computerId)
        {
            graphicsCard = new GraphicsCard(new Guid(), price, name, computerId);
            return this;
        }

        public GraphicsCard Build()
        {
            return this.graphicsCard;
        }
    }
}
