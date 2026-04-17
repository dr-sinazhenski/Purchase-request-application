using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.Dto
{
    public class CreateProductReqDto
    {
        public Guid? Id { get; set; }
        public string Name {  get; set; }
        public string Description { get; set; }
    }
}
