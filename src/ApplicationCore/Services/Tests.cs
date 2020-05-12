using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Text;
using ApplicationCore;

namespace ApplicationCore.Services
{
    public interface ITestsService
    {
        Task Test();
        Task<AuthResponse> LoginAsync(string remoteIp);
        Task RemoveSubsrcibesFromUserAsync();
        Task RemoveBillsFromUserAsync();
    }

    public class TestsService : ITestsService
    {
        private readonly IAppLogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IThirdPartyPayService _thirdPartyPayService;
        private readonly IBillsService _billsService;
        private readonly IPaysService _paysService;
        private readonly ISubscribesService _subscribesService;
        private readonly IUsersService _usersService;
        private readonly IAuthService _authService;

        public TestsService(IAppLogger logger, IOptions<AppSettings> appSettings, 
            IThirdPartyPayService thirdPartyPayService, IPaysService paysService, IBillsService billsService,
            ISubscribesService subscribesService, IUsersService usersService, IAuthService authService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            _thirdPartyPayService = thirdPartyPayService;
            _paysService = paysService;
            _billsService = billsService;
            _subscribesService = subscribesService;
            

            _usersService = usersService;
            _authService = authService;
        }

        User _testUser = null;
        User TestUser
        {
            get
            {
                if (_testUser == null) _testUser = _usersService.FindUserByEmailAsync("traders.com.tw@gmail.com").Result;
                return _testUser;
            }
        }

        public async Task Test()
        {
            await RemoveSubsrcibesFromUserAsync();
        }

        public async Task<AuthResponse> LoginAsync(string remoteIp)
        {
            var user = TestUser;
            var roles = await _usersService.GetRolesAsync(user);
            var oAuth = _authService.FindOAuthByProvider(user.Id, OAuthProvider.Google);
            return await _authService.CreateTokenAsync(remoteIp, user, oAuth, roles);
        }

        public async Task RemoveSubsrcibesFromUserAsync()
        {
            var subscribes = await _subscribesService.FetchByUserAsync(TestUser.Id);
            foreach (var subscribe in subscribes)
            {
                await RemoveSubsrcibeAsync(subscribe);
            }
        }

        async Task RemoveSubsrcibeAsync(Subscribe subscribe)
        {
            string userId = subscribe.UserId;
            
            await _usersService.RemoveSubscriberRoleAsync(userId);

            await _subscribesService.RemoveAsync(subscribe);

        }

        public async Task RemoveBillsFromUserAsync()
        {
            string userId = TestUser.Id;
            var bills = await _billsService.FetchByUserAsync(userId);

            foreach (var bill in bills)
            {
                _billsService.Remove(bill);
            }

        }

    }
}
