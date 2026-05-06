using Application.DTOs;
using Application.Services;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VinManagement.CleanArch.Controllers;

namespace VinManagement.CleanArch.Tests.Controllers;

public class VinsControllerTests
{
    private readonly Mock<IVinService> _serviceMock;
    private readonly VinsController _controller;

    public VinsControllerTests()
    {
        _serviceMock = new Mock<IVinService>();
        _controller = new VinsController(_serviceMock.Object);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedResult()
    {
        var dto = new CreateVinDto("1HGCM82633A004352", "Honda", "Accord", 2003);
        var response = new VinResponseDto(1, dto.Code, dto.VehicleMake, dto.VehicleModel, dto.Year, DateTime.UtcNow, null);

        _serviceMock.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Create(dto, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues!["id"]);

        var returnedValue = Assert.IsType<VinResponseDto>(createdResult.Value);
        Assert.Equal(dto.Code, returnedValue.Code);
    }

    [Fact]
    public async Task Create_InvalidRequest_ReturnsBadRequest()
    {
        var dto = new CreateVinDto("1HGCM82633A004352", "Honda", "Accord", 2003);

        _serviceMock.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Invalid VIN code"));

        await Assert.ThrowsAsync<BadRequestException>(() => _controller.Create(dto, CancellationToken.None));
    }

    [Fact]
    public async Task GetById_ExistingVin_ReturnsOkResult()
    {
        var response = new VinResponseDto(1, "1HGCM82633A004352", "Honda", "Accord", 2003, DateTime.UtcNow, null);

        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetById(1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = Assert.IsType<VinResponseDto>(okResult.Value);
        Assert.Equal(1, returnedValue.Id);
        Assert.Equal("1HGCM82633A004352", returnedValue.Code);
    }

    [Fact]
    public async Task GetById_NonExistingVin_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Vin", 999));

        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(999, CancellationToken.None));
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithList()
    {
        var vins = new List<VinResponseDto>
        {
            new(1, "1HGCM82633A004352", "Honda", "Accord", 2003, DateTime.UtcNow, null),
            new(2, "2HGCM82633A004353", "Toyota", "Camry", 2005, DateTime.UtcNow, null)
        };

        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vins);

        var result = await _controller.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<VinResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task Update_ExistingVin_ReturnsOkResult()
    {
        var dto = new UpdateVinDto("1HGCM82633A004352", "Honda", "Civic", 2024);
        var response = new VinResponseDto(1, dto.Code, dto.VehicleMake, dto.VehicleModel, dto.Year, DateTime.UtcNow, null);

        _serviceMock.Setup(s => s.UpdateAsync(1, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Update(1, dto, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = Assert.IsType<VinResponseDto>(okResult.Value);
        Assert.Equal(1, returnedValue.Id);
        Assert.Equal("Civic", returnedValue.VehicleModel);
    }

    [Fact]
    public async Task Update_NonExistingVin_ThrowsNotFoundException()
    {
        var dto = new UpdateVinDto("1HGCM82633A004352", "Honda", "Civic", 2024);

        _serviceMock.Setup(s => s.UpdateAsync(999, dto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Vin", 999));

        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Update(999, dto, CancellationToken.None));
    }

    [Fact]
    public async Task Delete_ExistingVin_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingVin_ThrowsNotFoundException()
    {
        _serviceMock.Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Vin", 999));

        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Delete(999, CancellationToken.None));
    }
}
