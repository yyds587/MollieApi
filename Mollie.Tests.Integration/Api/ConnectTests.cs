﻿using Mollie.Api.Client;
using Mollie.Api.Models.Connect;
using Mollie.Tests.Integration.Framework;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mollie.Tests.Integration.Api {
    [TestFixture]
    public class ConnectTests : BaseMollieApiTestClass {
        [Test]
        public void GetAuthorizationUrl_WithSingleScope_GeneratesAuthorizationUrl() {
            // Given: We create a new connect client
            ConnectClient connectClient = new ConnectClient(this.ClientId, this.ClientSecret);

            // When: We get the authorization URL
            string authorizationUrl = connectClient.GetAuthorizationUrl("abcde", new List<string>() { AppPermissions.PaymentsRead });

            // Then: 
            string expectedUrl = $"https://www.mollie.com/oauth2/authorize?client_id={this.ClientId}&state=abcde&scope=payments.read&response_type=code&approval_prompt=auto";
            Assert.AreEqual(expectedUrl, authorizationUrl);
        }

        [Test]
        public void GetAuthorizationUrl_WithMultipleScopes_GeneratesAuthorizationUrl() {
            // Given: We create a new connect client
            ConnectClient connectClient = new ConnectClient(this.ClientId, this.ClientSecret);

            // When: We get the authorization URL
            string authorizationUrl = connectClient.GetAuthorizationUrl("abcdef", new List<string>() {
                AppPermissions.PaymentsRead, 
                AppPermissions.PaymentsWrite, 
                AppPermissions.ProfilesRead, 
                AppPermissions.ProfilesWrite
            });

            // Then: 
            string expectedUrl = $"https://www.mollie.com/oauth2/authorize?client_id={this.ClientId}" +
                                 $"&state=abcdef&scope=payments.read+payments.write+profiles.read+profiles.write&response_type=code&approval_prompt=auto";
            Assert.AreEqual(expectedUrl, authorizationUrl);
        }
        
        [Test]
        [Ignore("We can only test this in debug mode, because we login to the mollie dashboard and login to get the auth token")]
        public async Task GetAccessTokenAsync_WithValidTokenRequest_ReturnsAccessToken() {
            // Given: We fetch create a token request
            string authCode = "abcde"; // Set a valid access token here
            ConnectClient connectClient = new ConnectClient(this.ClientId, this.ClientSecret);
            TokenRequest tokenRequest = new TokenRequest(authCode, DefaultRedirectUrl);

            // When: We request the auth code
            TokenResponse tokenResponse = await connectClient.GetAccessTokenAsync(tokenRequest);

            // Then: The access token should not be null
            Assert.IsFalse(string.IsNullOrEmpty(tokenResponse.AccessToken));
        }

        [Test]
        [Ignore("We can only test this in debug mode, because we need a valid access token")]
        public async Task RevokeAccessTokenAsync_WithValidToken_DoesNotThrowError() {
            // Given: We create a revoke token request
            string accessToken = "abcde";
            ConnectClient connectClient = new ConnectClient(this.ClientId, this.ClientSecret);
            RevokeTokenRequest revokeTokenRequest = new RevokeTokenRequest() {
                TokenTypeHint = TokenType.AccessToken,
                Token = accessToken
            };

            // When: we send the request
            await connectClient.RevokeTokenAsync(revokeTokenRequest);
        }
    }
}
