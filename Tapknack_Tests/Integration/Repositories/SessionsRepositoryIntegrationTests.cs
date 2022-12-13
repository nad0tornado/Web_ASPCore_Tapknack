﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tapknack_Server.Models;
using Tapknack_Server.Providers;
using Tapknack_Server.Repositories;
using Xunit;

namespace Tapknack_Tests.Integration.Repositories
{
  public class SessionsRepositoryIntegrationTests
  {
    [Fact]
    public async Task IntegrationTest()
    {
      var testUser = await TestHelpers.CreateTestUser();
      var sessionsRepo = new SessionsRepository();

      var token = Guid.NewGuid();
      var accessToken = Guid.NewGuid();
      var expiry = DateTime.UtcNow;
      var accessExpiry = DateTime.UtcNow;

      var session = await sessionsRepo.AddAsync(new Session()
      {
        UserId = testUser.Id,
        Token = token,
        Expiry = expiry,
        AccessToken = accessToken,
        AccessExpiry = accessExpiry
      });

      Assert.NotNull(session);

      var sessionById = await sessionsRepo.GetSingleAsync(session.Id);
      Assert.NotNull(sessionById);
      Assert.Equal(session.Id, sessionById.Id);

      var sessionByUserId = await sessionsRepo.GetByUserIdAsync(testUser.Id);
      Assert.NotNull(sessionByUserId);
      Assert.Equal(testUser.Id, sessionByUserId.UserId);

      var sessionByToken = await sessionsRepo.GetByTokenAsync(token);
      Assert.NotNull(sessionByToken);
      Assert.Equal(testUser.Id, sessionByToken.UserId);
      Assert.Equal(token, sessionByToken.Token);

      var sessionByAccessToken = await sessionsRepo.GetByAccessTokenAsync(accessToken);
      Assert.NotNull(sessionByAccessToken);
      Assert.Equal(testUser.Id, sessionByAccessToken.UserId);
      Assert.Equal(accessToken, sessionByAccessToken.AccessToken);

      // .. also tests the "Update" method
      var accessTokenNew = Guid.NewGuid();
      var updatedSessionCount = await sessionsRepo.UpdateAsync(sessionByAccessToken with
      {
        AccessToken = accessTokenNew,
        AccessExpiry = accessExpiry
      });

      Assert.Equal(1, updatedSessionCount);

      var updatedAccessTokenSession = await sessionsRepo.GetSingleAsync(sessionByAccessToken.Id);
      Assert.NotNull(updatedAccessTokenSession);
      Assert.Equal(testUser.Id, updatedAccessTokenSession.UserId);
      Assert.Equal(accessTokenNew, updatedAccessTokenSession.AccessToken);
    }
  }
}
