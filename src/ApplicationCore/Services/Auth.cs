using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
	public interface IAuthService
	{
		Task<AuthResponse> CreateTokenAsync(string ipAddress, double daysToExpire, User user, OAuth oAuth, IList<string> roles = null);
		Task CreateUpdateUserOAuthAsync(string userId, OAuth oAuth);
	}

	public class AuthService : IAuthService
	{
		private readonly IJwtFactory _jwtFactory;
		private readonly ITokenFactory _tokenFactory;
		private readonly IJwtTokenValidator _jwtTokenValidator;

		private readonly IDefaultRepository<RefreshToken> _refreshTokenRepository;
		private readonly IDefaultRepository<OAuth> _oAuthRepository;

		public AuthService(IJwtFactory jwtFactory, ITokenFactory tokenFactory, IJwtTokenValidator jwtTokenValidator,
			IDefaultRepository<RefreshToken> refreshTokenRepository, IDefaultRepository<OAuth> oAuthRepository)
		{
			_jwtFactory = jwtFactory;
			_tokenFactory = tokenFactory;
			_jwtTokenValidator = jwtTokenValidator;

			_refreshTokenRepository = refreshTokenRepository;
			_oAuthRepository = oAuthRepository;
		}

		public async Task<AuthResponse> CreateTokenAsync(string ipAddress, double daysToExpire, User user, OAuth oAuth, IList<string> roles = null)
		{
			var accessToken = await _jwtFactory.GenerateEncodedToken(user.Id, user.UserName, roles);
			var refreshToken = _tokenFactory.GenerateToken();

			await SetRefreshTokenAsync(ipAddress, daysToExpire, user, refreshToken);

			return new AuthResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}

		public async Task CreateUpdateUserOAuthAsync(string userId, OAuth oAuth)
		{
			var spec = new OAuthFilterSpecification(userId, oAuth.Provider);
			var exist = _oAuthRepository.GetSingleBySpec(spec);

			if (exist != null)
			{
				exist.OAuthId = oAuth.OAuthId;
				await _oAuthRepository.UpdateAsync(exist);
			}
			else
			{
				await _oAuthRepository.AddAsync(oAuth);
			}

		}

		async Task SetRefreshTokenAsync(string ipAddress, double daysToExpire, User user, string token)
		{
			var expires = DateTime.UtcNow.AddDays(daysToExpire);

			var exist = GetRefreshToken(user.Id);
			if (exist != null)
			{
				exist.Token = token;
				exist.Expires = expires;
				exist.RemoteIpAddress = ipAddress;

				await _refreshTokenRepository.UpdateAsync(exist);
			}
			else
			{
				var refreshToken = new RefreshToken
				{
					Token = token,
					Expires = expires,
					UserId = user.Id,
					RemoteIpAddress = ipAddress
				};

				await _refreshTokenRepository.AddAsync(refreshToken);

			}



		}

		RefreshToken GetRefreshToken(string userId)
		{
			var spec = new RefreshTokenFilterSpecification(userId);
			return _refreshTokenRepository.GetSingleBySpec(spec);

		}


		bool HasValidRefreshToken(string userId, string token)
		{
			var entity = GetRefreshToken(userId);
			if (entity == null) return false;

			return entity.Token == token && entity.Active;

		}

	}
}
