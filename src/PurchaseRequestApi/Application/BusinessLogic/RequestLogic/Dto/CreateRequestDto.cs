using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.Dto
{
    public class CreateRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid RequestTypeId { get; set; }
        public Dictionary<Guid, int> ProductIdAmount { get; set; } = new Dictionary<Guid, int>();
    }
}
