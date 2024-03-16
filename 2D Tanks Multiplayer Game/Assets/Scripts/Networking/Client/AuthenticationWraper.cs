using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Networking.Client
{
//  when it's called tries to auth the instance
    public static class AuthenticationWraper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxRetries = 5)
        {
            if (AuthState == AuthState.Authenticated)
                return AuthState;

            if (AuthState == AuthState.Authenticating)
            {
                Debug.LogWarning("Already authenticating!");
                await Authenticating();
                return AuthState;
            }

            await SignInAnonymouslyAsync(maxRetries);

            return AuthState;
        }

        private static async Task<AuthState> Authenticating()
        {
            while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        public static async Task SignInAnonymouslyAsync(int maxRetries)
        {
            AuthState = AuthState.Authenticating;
            int retries = 0;

            while (AuthState == AuthState.Authenticating && retries < maxRetries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthState = AuthState.Authenticated;
                        break;
                    }

                }
                catch (AuthenticationException e)
                {
                    Debug.LogError(e);
                    AuthState = AuthState.Error;
                }
                catch (RequestFailedException ex)
                {
                    Debug.LogError(ex);
                    AuthState = AuthState.Error;
                }

                retries++;
                await Task.Delay(1000);
            }

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not sign in successfuly after {retries} retries");
                AuthState = AuthState.TimeOut;
            }
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}