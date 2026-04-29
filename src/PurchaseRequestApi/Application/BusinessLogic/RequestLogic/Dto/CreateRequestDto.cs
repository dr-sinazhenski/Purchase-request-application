using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.Dto
{
    public class CreateRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid requestTypeId { get; set; }
    }
}
