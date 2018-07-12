using System;
using System.Collections.Generic;
using System.Text;

namespace API.Contracts.Dtos
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
