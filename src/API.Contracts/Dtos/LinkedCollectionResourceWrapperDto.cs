using System;
using System.Collections.Generic;
using System.Text;

namespace API.Contracts.Dtos
{
    public class LinkedCollectionResourceWrapperDto<T> : LinkedResourceBaseDto where T : LinkedResourceBaseDto
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapperDto(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
