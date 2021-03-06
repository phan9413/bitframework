﻿using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.Core.Models;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Bit.Http.Contracts
{
    public class Token
    {
        public static implicit operator Token(Dictionary<string, string> props)
        {
            if (props == null)
                throw new ArgumentNullException(nameof(props));

            Token token = new Token
            {
                access_token = props[nameof(access_token)],
                expires_in = Convert.ToInt64(props[nameof(expires_in)], CultureInfo.InvariantCulture),
                token_type = props[nameof(token_type)]
            };

            if (props.ContainsKey(nameof(token.id_token)))
                token.id_token = props[nameof(token.id_token)];

            if (!props.ContainsKey(nameof(token.login_date)))
                token.login_date = DefaultDateTimeProvider.Current.GetCurrentUtcDateTime();
            else
                token.login_date = Convert.ToDateTime(props[nameof(login_date)], CultureInfo.InvariantCulture);

            return token;
        }

        public static implicit operator Token(TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
                throw new ArgumentNullException(nameof(tokenResponse));

            return new Token
            {
                access_token = tokenResponse.AccessToken,
                expires_in = tokenResponse.ExpiresIn,
                login_date = DefaultDateTimeProvider.Current.GetCurrentUtcDateTime(),
                token_type = tokenResponse.TokenType
            };
        }

        public string access_token { get; set; } = default!;

        public string? id_token { get; set; }

        public string token_type { get; set; } = default!;

        public long expires_in { get; set; }

        public DateTimeOffset? login_date { get; set; }
    }

    public interface ISecurityService : ISecurityServiceBase
    {
        Task<Token> LoginWithCredentials(string userName, string password, string client_id, string client_secret, string[]? scopes = null, IDictionary<string, string?>? acr_values = null, CancellationToken cancellationToken = default);

        Task<Token> Login(object? state = null, string? client_id = null, IDictionary<string, string?>? acr_values = null, CancellationToken cancellationToken = default);

        Task<Token?> GetCurrentTokenAsync(CancellationToken cancellationToken = default);

        Token? GetCurrentToken();

        Task<BitJwtToken> GetBitJwtTokenAsync(CancellationToken cancellationToken);

        BitJwtToken GetBitJwtToken();
    }
}
