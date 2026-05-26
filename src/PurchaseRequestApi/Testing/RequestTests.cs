using Application.BusinessLogic.RequestLogic.CreateRequest;
using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestLogic.GetAllRequests;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing;

[TestFixture]
public class RequestTests : HandlerTestBase
{
    protected override async Task SeedAsync(AppDbContext db)
    {
        DBSeeder.SeedRequestType(db, name: "Bug Report");
    }

    [Test]
    public async Task Add_adds_request_successfully()
    {

        var type = await Database.RequestTypes.FirstOrDefaultAsync();
        var dto = new CreateRequestDto
        {
            Title = "123",
            Description = "456",
            RequestTypeId = type.Id,
        };

        var responce = await Mediator.Send(new CreateRequestCommand(dto));

        Assert.That(responce.IsSuccess, Is.EqualTo(true));

        var count = await Database.Requests.CountAsync(c => c.Title == dto.Title);
        Assert.That(count, Is.EqualTo(1));
    }
}