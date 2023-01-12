using Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Services.Tests;

public class ServiceObjectServiceTests
{
    private readonly Mock<IRepositoryManager> _repositoryManagerMock;

    public ServiceObjectServiceTests()
    {
        var managerMock = new Mock<IRepositoryManager>();
        managerMock.Setup(x => x.ServiceObjectRepository)
                   .Returns(new Mock<IServiceObjectRepository>().Object);

        managerMock.Setup(x => x.UnitOfWork)
                   .Returns(new Mock<IUnitOfWork>().Object);

        _repositoryManagerMock = managerMock;
    }

    [Fact]
    public async Task Can_Create()
    {
        var name = "test";
        var amount = 1;
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var result = await service.CreateAsync(name, amount);

        Assert.NotNull(result);
        Assert.IsType<ServiceObjectDto>(result);
        Assert.True(Guid.TryParse(result.Id, out var id));
        Assert.Equal(name, result.Name);
        Assert.Equal(amount, result.Amount);
        _repositoryManagerMock.Verify(x => x.ServiceObjectRepository.Add(It.IsAny<ServiceObject>()), Times.Once);
        _repositoryManagerMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_Throws_ArgumentException_If_Amount_Less_Than0()
    {
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var function = () => service.CreateAsync("any", -1);

        await Assert.ThrowsAsync<ArgumentException>(function);
    }

    [Theory]
    [InlineData("test", 100)]
    [InlineData("test", 0)]
    public async Task Can_Update_WithoutNulls(string? name, int? amount)
    {
        var id = Guid.NewGuid();
        ConfigureRepositoryMock(id);
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var result = await service.UpdateAsync(id, name, amount);

        Assert.NotNull(result);
        Assert.IsType<ServiceObjectDto>(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(amount, result.Amount);
        _repositoryManagerMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null, 100)]
    [InlineData("test", null)]
    [InlineData(null, null)]
    public async Task Can_Update_WithNulls(string? name, int? amount)
    {
        var id = Guid.NewGuid();
        var item = ConfigureRepositoryMock(id);
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var result = await service.UpdateAsync(id, name, amount);

        Assert.NotNull(result);
        Assert.IsType<ServiceObjectDto>(result);
        Assert.Equal(item.Name, result.Name);
        Assert.Equal(item.Amount, result.Amount);
        _repositoryManagerMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_Throws_ArgumentException_If_Amount_Less_Than0()
    {
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var function = () => service.UpdateAsync(Guid.NewGuid(), "any", -1);

        await Assert.ThrowsAsync<ArgumentException>(function);
    }

    [Fact]
    public async Task Update_Throws_ServiceObjectNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryManagerMock.Setup(x => x.ServiceObjectRepository
                             .ExecuteQuery(It.IsAny<Func<IQueryable<ServiceObject>, Task<ServiceObject>>>(),
                                           It.IsAny<CancellationToken>()))
                             .ReturnsAsync((ServiceObject)null);
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var function = () => service.UpdateAsync(id, "any", 0);

        await Assert.ThrowsAsync<ServiceObjectNotFoundException>(function);
    }

    [Fact]
    public async Task Can_Booking_With_Amount_More_ThanAvailable()
    {
        var id = Guid.NewGuid();
        var item = ConfigureRepositoryMock(id);
        ServiceObjectService.ResetRecords();
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var (Ok, AmountLeft, Error) = await service.BookingAsync(id, 999);

        Assert.False(Ok);
        Assert.Equal(item.Amount, AmountLeft);
        Assert.False(string.IsNullOrEmpty(Error));
    }

    [Fact]
    public async Task Can_Booking_With_Amount_Less_ThanAvailable()
    {
        var id = Guid.NewGuid();
        var amount = 25;
        var item = ConfigureRepositoryMock(id);
        ServiceObjectService.ResetRecords();
        var service = new ServiceObjectService(_repositoryManagerMock.Object);

        var (Ok, AmountLeft, Error) = await service.BookingAsync(id, amount);
        var records = ServiceObjectService.GetAllRecords();

        Assert.True(Ok);
        Assert.Equal(item.Amount - amount, AmountLeft);
        Assert.True(string.IsNullOrEmpty(Error));
        Assert.NotNull(records);
        Assert.Equal(records.First().ObjectName, item.Name);
    }

    private ServiceObject ConfigureRepositoryMock(Guid id)
    {
        var item = new ServiceObject
        {
            Id = id.ToString(),
            Amount = 50,
            Name = "Not a test"
        };
        _repositoryManagerMock.Setup(x => x.ServiceObjectRepository
                              .ExecuteQuery(It.IsAny<Func<IQueryable<ServiceObject>, Task<ServiceObject>>>(),
                                            It.IsAny<CancellationToken>()))
                              .ReturnsAsync(item);

        return item;
    }
}
