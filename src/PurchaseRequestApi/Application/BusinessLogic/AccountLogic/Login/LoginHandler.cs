using Application.BusinessLogic.AccountLogic.Dto;
using Application.Options;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.BusinessLogic.AccountLogic.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResDto>>
    {
        private readonly ILogger<LoginHandler> _logger;
        private readonly AppDbContext _dbContext;
        private readonly JwtOptions _jwtOptions;

        public LoginHandler(AppDbContext dbContext, ILogger<LoginHandler> logger, IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _dbContext = dbContext;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<Result<LoginResDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Login}", command.dto.Login);

            var account = await _dbContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Login == command.dto.Login, cancellationToken);

            if (account == null || account.Password != command.dto.Password)
            {
                var err = new Error(401, "Invalid login or password");
                _logger.LogError(err.ToString());
                return Result<LoginResDto>.Failure(err);
            }

            var roles = account.Role.Select(r => r.Name).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Login successful for {Login}", command.dto.Login);

            return Result<LoginResDto>.Success(new LoginResDto
            {
                Token = tokenString,
                Name = account.Name,
                Roles = roles
            });
        }
    }
}