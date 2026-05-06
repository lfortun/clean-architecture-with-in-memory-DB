using Application.DTOs;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Moq;

namespace VinManagement.CleanArch.Tests.Application.Services;

public class VinServiceTests
{
    private readonly Mock<IVinRepository> _repositoryMock;
    private readonly Mock<IVinValidator> _validatorMock;
    private readonly VinService _service;

    public VinServiceTests()
    {
        _repositoryMock = new Mock<IVinRepository>();
        _validatorMock = new Mock<IVinValidator>();
        _validatorMock.Setup(v => v.IsValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _service = new VinService(_repositoryMock.Object, new[] { _validatorMock.Object });
    }

    [Fact]
    public async Task CreateAsync_ValidVin_ReturnsResponseDto()
    {
        var dto = new CreateVinDto("1HGCM82633A004352", "Honda", "Accord", 2003);
        var createdVin = new Vin(dto.Code, dto.VehicleMake, dto.VehicleModel, dto.Year);

        _repositoryMock.Setup(r => r.CodeExistsAsync(dto.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdVin);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Code, result.Code);
        Assert.Equal(dto.VehicleMake, result.VehicleMake);
        Assert.Equal(dto.VehicleModel, result.VehicleModel);
        Assert.Equal(dto.Year, result.Year);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidVinCode_ThrowsBadRequestException()
    {
        var dto = new CreateVinDto("SHORT", "Honda", "Accord", 2003);

        _validatorMock.Setup(v => v.IsValidAsync(dto.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(dto));

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ThrowsBadRequestException()
    {
        var dto = new CreateVinDto("1HGCM82633A004352", "Honda", "Accord", 2003);

        _repositoryMock.Setup(r => r.CodeExistsAsync(dto.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(dto));

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingVin_ReturnsResponseDto()
    {
        var vin = new Vin("1HGCM82633A004352", "Honda", "Accord", 2003);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vin);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(vin.Code, result.Code);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingVin_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vin?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfVins()
    {
        var vins = new List<Vin>
        {
            new("1HGCM82633A004352", "Honda", "Accord", 2003),
            new("2HGCM82633A004353", "Toyota", "Camry", 2005)
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vins);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("1HGCM82633A004352", result[0].Code);
        Assert.Equal("2HGCM82633A004353", result[1].Code);
    }

    [Fact]
    public async Task UpdateAsync_ExistingVin_ReturnsUpdatedDto()
    {
        var vin = new Vin("1HGCM82633A004352", "Honda", "Accord", 2003);
        var dto = new UpdateVinDto("1HGCM82633A004352", "Honda", "Civic", 2024);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vin);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vin);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Honda", result.VehicleMake);
        Assert.Equal("Civic", result.VehicleModel);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_InvalidVinCode_ThrowsBadRequestException()
    {
        var vin = new Vin("1HGCM82633A004352", "Honda", "Accord", 2003);
        var dto = new UpdateVinDto("SHORT", "Honda", "Civic", 2024);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vin);
        _validatorMock.Setup(v => v.IsValidAsync(dto.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateAsync(1, dto));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingVin_ThrowsNotFoundException()
    {
        var dto = new UpdateVinDto("1HGCM82633A004352", "Honda", "Civic", 2024);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vin?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, dto));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Vin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingVin_DeletesSuccessfully()
    {
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingVin_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
