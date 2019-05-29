using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcare.Common.Crypto
{
    public class Cryptolib
    {
        string clientId = "69fda8c4-e6b7-492f-b283-bea4f74c46d7";
        string clientSecret = "QgVaVReJwuTxJ1pblkuPARdppXGAgqkSM+6M8zq1X2g=";
        KeyVaultClient cryptoHelper;
        KeyBundle keyBundle;
        string publicKey;

        public Cryptolib(string keyIdentifier)
        {
            keyBundle = GetKey("https://eapservice20181220131146.vault.azure.net/keys/Statecommonkey/22eacae588064166b2fe87dc1dc739c6").GetAwaiter().GetResult();
            publicKey = Convert.ToBase64String(keyBundle.Key.N);
        }

        private async Task<KeyBundle> GetKey(string keyIdentifier)
        {
            cryptoHelper = new KeyVaultClient(GetToken);
            var secureKey = await cryptoHelper.GetKeyAsync(keyIdentifier);

            return secureKey;
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(
                clientId,
                clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

        public async Task<string> EncryptString(string plainText)
        {
            var result = await cryptoHelper.EncryptAsync(keyBundle.KeyIdentifier.Identifier, "RSA-OAEP-256", Encoding.Unicode.GetBytes(plainText));
            return Convert.ToBase64String(result.Result);
        }

        public async Task<string> DecryptString(string chipperText)
        {
            var result = await cryptoHelper.DecryptAsync(keyBundle.KeyIdentifier.Identifier, "RSA-OAEP-256", Convert.FromBase64String(chipperText));
            return Encoding.Unicode.GetString(result.Result);
        }

    }
}
