using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using AgencySettlement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AgencySettlement.Application.Features.Commands.RegisterCommand;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var phoneNumber = request.PhoneNumber.Trim();

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Last name is required.");

        var existingUser =
            await _userRepository.GetByPhoneNumberAsync(
                phoneNumber,
                cancellationToken);

        if (existingUser is not null)
            throw new InvalidOperationException(
                "A user with this phone number already exists.");

        var user = new User
        {
            PhoneNumber = phoneNumber,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        return new RegisterResponse(
            user.Id,
            user.PhoneNumber,
            user.FirstName,
            user.LastName);
    }
}