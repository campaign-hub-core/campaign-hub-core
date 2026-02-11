using CampaignHub.Application.Customers;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Entities.Enum;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.Customers;

public class CustomerServiceTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new CustomerService(_customerRepository, _userRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCustomer_WhenUserExistsAndActive()
    {
        var user = new User("John", "john@test.com", "hash");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);
        _customerRepository.AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Customer>());
        var input = new CreateCustomerInput(" Client A ", user.Id, " Notes ", CustomerTypeEnum.Lead);

        var result = await _sut.CreateAsync(input);

        Assert.Equal("Client A", result.Name);
        Assert.Equal("Notes", result.Observation);
        Assert.Equal(CustomerTypeEnum.Lead, result.CustomerType);
        Assert.Equal(CustomerStatusEnum.Active, result.CustomerStatus);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var input = new CreateCustomerInput("Client", "nonexistent", null, CustomerTypeEnum.Lead);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserInactive()
    {
        var user = new User("John", "john@test.com", "hash");
        user.Disable();
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);
        var input = new CreateCustomerInput("Client", user.Id, null, CustomerTypeEnum.Lead);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(input));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenExistsAndNotInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.GetByIdAsync(customer.Id);

        Assert.NotNull(result);
        Assert.Equal(customer.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _customerRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.GetByIdAsync(customer.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer_WhenExistsAndNotInactive()
    {
        var customer = new Customer("Client A", "user-1", "Old", CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.UpdateAsync(customer.Id, new UpdateCustomerInput(" Client B ", " New "));

        Assert.NotNull(result);
        Assert.Equal("Client B", result!.Name);
        Assert.Equal("New", result.Observation);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.UpdateAsync(customer.Id, new UpdateCustomerInput("Name", null));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDisableCustomer_WhenExistsAndNotInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.DeleteAsync(customer.Id);

        Assert.True(result);
        Assert.Equal(CustomerStatusEnum.Inactive, customer.CustomerStatus);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAlreadyInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.DeleteAsync(customer.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task EnableAsync_ShouldSetStatusActive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.EnableAsync(customer.Id);

        Assert.NotNull(result);
        Assert.Equal(CustomerStatusEnum.Active, result!.CustomerStatus);
    }

    [Fact]
    public async Task EnableAsync_ShouldReturnNull_WhenNotFound()
    {
        _customerRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var result = await _sut.EnableAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task PauseAsync_ShouldSetStatusSuspended()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.PauseAsync(customer.Id);

        Assert.NotNull(result);
        Assert.Equal(CustomerStatusEnum.Suspended, result!.CustomerStatus);
    }

    [Fact]
    public async Task PauseAsync_ShouldReturnNull_WhenInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.PauseAsync(customer.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DisableAsync_ShouldSetStatusInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.DisableAsync(customer.Id);

        Assert.NotNull(result);
        Assert.Equal(CustomerStatusEnum.Inactive, result!.CustomerStatus);
    }

    [Fact]
    public async Task DisableAsync_ShouldReturnNull_WhenAlreadyInactive()
    {
        var customer = new Customer("Client A", "user-1", null, CustomerTypeEnum.Lead);
        customer.Disable();
        _customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>())
            .Returns(customer);

        var result = await _sut.DisableAsync(customer.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnMatchingCustomers()
    {
        var customers = new List<Customer>
        {
            new("Client A", "user-1", null, CustomerTypeEnum.Lead),
            new("Client AB", "user-2", null, CustomerTypeEnum.Ecommerce)
        };
        _customerRepository.GetByNameAsync("Client", Arg.Any<CancellationToken>())
            .Returns(customers);

        var result = await _sut.GetByNameAsync("Client");

        Assert.Equal(2, result.Count());
    }
}
