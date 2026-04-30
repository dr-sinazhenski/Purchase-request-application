using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.Dto
{
    public class CreateProductDto
    {
        public Guid? Id { get; set; }
        required public string Name {  get; set; }
        required public string Description { get; set; }
        public List<Guid> RequestTypeIds { get; set; } = new List<Guid>();
    }
}
