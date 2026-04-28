using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.Dto
{
    public class CreateProductReqDto
    {
        public Guid? Id { get; set; }
        required public string Name {  get; set; }
        required public string Description { get; set; }
    }
}
