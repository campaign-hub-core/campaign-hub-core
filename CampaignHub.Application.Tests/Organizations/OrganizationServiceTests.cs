using CampaignHub.Application.Organizations;
using CampaignHub.Domain.Entities;
using CampaignHub.Domain.Interfaces;
using NSubstitute;

namespace CampaignHub.Application.Tests.Organizations;

public class OrganizationServiceTests
{
    private readonly IOrganizationRepository _orgRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrganizationService _sut;

    public OrganizationServiceTests()
    {
        _orgRepository = Substitute.For<IOrganizationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new OrganizationService(_orgRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateOrganization()
    {
        var input = new CreateOrganizationInput(" My Org ");
        _orgRepository.AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Organization>());

        var result = await _sut.CreateAsync(input);

        Assert.Equal("My Org", result.Name);
        Assert.True(result.Active);
        await _orgRepository.Received(1).AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrganization_WhenExistsAndActive()
    {
        var org = new Organization("Test Org");
        _orgRepository.GetByIdAsync(org.Id, Arg.Any<CancellationToken>())
            .Returns(org);

        var result = await _sut.GetByIdAsync(org.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Org", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _orgRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var result = await _sut.GetByIdAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenInactive()
    {
        var org = new Organization("Test Org");
        org.Disable();
        _orgRepository.GetByIdAsync(org.Id, Arg.Any<CancellationToken>())
            .Returns(org);

        var result = await _sut.GetByIdAsync(org.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnMatchingOrganizations()
    {
        var orgs = new List<Organization> { new("Org A"), new("Org B") };
        _orgRepository.GetByNameAsync("Org", Arg.Any<CancellationToken>())
            .Returns(orgs);

        var result = await _sut.GetByNameAsync("Org");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateName_WhenExistsAndActive()
    {
        var org = new Organization("Old Name");
        _orgRepository.GetByIdAsync(org.Id, Arg.Any<CancellationToken>())
            .Returns(org);

        var result = await _sut.UpdateAsync(org.Id, new UpdateOrganizationInput(" New Name "));

        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        _orgRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var result = await _sut.UpdateAsync("nonexistent", new UpdateOrganizationInput("Name"));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDisableOrganization_WhenExistsAndActive()
    {
        var org = new Organization("Test Org");
        _orgRepository.GetByIdAsync(org.Id, Arg.Any<CancellationToken>())
            .Returns(org);

        var result = await _sut.DeleteAsync(org.Id);

        Assert.True(result);
        Assert.False(org.Active);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _orgRepository.GetByIdAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var result = await _sut.DeleteAsync("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAlreadyInactive()
    {
        var org = new Organization("Test Org");
        org.Disable();
        _orgRepository.GetByIdAsync(org.Id, Arg.Any<CancellationToken>())
            .Returns(org);

        var result = await _sut.DeleteAsync(org.Id);

        Assert.False(result);
    }
}
