using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.Dto
{
    public class RejectRequestDto
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
        public bool IsFinal { get; set; }
    }
}
