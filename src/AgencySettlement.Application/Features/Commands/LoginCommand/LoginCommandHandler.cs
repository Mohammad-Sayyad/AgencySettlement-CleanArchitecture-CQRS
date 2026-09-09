using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Application.Features.Commands.LoginCommand
{
    public sealed class LoginCommandHandler
     : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly PasswordHasher<Domain.Entities.User> _passwordHasher;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;

            _passwordHasher =
                new PasswordHasher<Domain.Entities.User>();
        }

        public async Task<LoginResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new UnauthorizedAccessException(
                    "شماره موبایل الزامی است.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new UnauthorizedAccessException(
                    "رمز عبور الزامی است.");

            var phoneNumber =
                NormalizePhoneNumber(request.PhoneNumber);

            var user = await _userRepository
                .GetByPhoneNumberAsync(
                    phoneNumber,
                    cancellationToken);

            if (user is null)
                throw new UnauthorizedAccessException(
                    "شماره موبایل یا رمز عبور اشتباه است.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException(
                    "حساب کاربری غیرفعال است.");

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "شماره موبایل یا رمز عبور اشتباه است.");
            }

            user.LastLoginAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(
                user,
                cancellationToken);

            var token =
                _jwtTokenGenerator.GenerateToken(user);

            var expiresAt =
                DateTime.UtcNow.AddHours(8);

            return new LoginResponse(
                token,
                expiresAt,
                user.Id,
                user.PhoneNumber,
                user.FirstName,
                user.LastName);
        }

        private static string NormalizePhoneNumber(
            string phoneNumber)
        {
            return phoneNumber
                .Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("+98", "0");
        }
    }
}
