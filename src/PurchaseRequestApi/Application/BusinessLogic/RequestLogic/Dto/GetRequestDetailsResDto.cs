using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.Dto
{
    public class GetRequestDetailsResDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RequestTypeResDto RequestType { get; set; }
        public string Status { get; set; }
        public string? RejectionCommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RequesterId { get; set; }

        public List<ProductListItemDto> Products { get; set; }
    }
}
