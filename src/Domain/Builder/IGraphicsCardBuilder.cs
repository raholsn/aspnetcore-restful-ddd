using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builder
{
    public interface IGraphicsCardBuilder
    {
        GraphicsCardBuilder CreateGraphicsCard(double price, string name, Guid computerId);
        GraphicsCardBuilder UpdateGraphicsCard(Guid id, double price, string name, Guid computerId);
        GraphicsCard Build();
    }
}
