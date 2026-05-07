using Application.DTOs;
using FluentValidation;

namespace VinManagement.CleanArch.Validators;

public class UpdateVinDtoValidator : AbstractValidator<UpdateVinDto>
{
    public UpdateVinDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("VIN code is required.")
            .Length(17).WithMessage("VIN code must be exactly 17 characters.");

        RuleFor(x => x.VehicleMake)
            .NotEmpty().WithMessage("Vehicle make is required.")
            .MaximumLength(100).WithMessage("Vehicle make must not exceed 100 characters.");

        RuleFor(x => x.VehicleModel)
            .NotEmpty().WithMessage("Vehicle model is required.")
            .MaximumLength(100).WithMessage("Vehicle model must not exceed 100 characters.");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be a positive value.")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage($"Year cannot be greater than {DateTime.Now.Year}.")
            .When(x => x.Year.HasValue);
    }
}
