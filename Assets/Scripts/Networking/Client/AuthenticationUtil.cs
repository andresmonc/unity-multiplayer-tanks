using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationUtil
{

    public static AuthenticationState AuthenticationState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuth(int maxRetries = 5)
    {
        if (AuthenticationState == AuthenticationState.Authenticated)
        {
            return AuthenticationState;
        }
        if (AuthenticationState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            return await Authenticating();
        }

        await SignInAnonymouslyAsync(maxRetries);
        return AuthenticationState;
    }

    private static async Task<AuthenticationState> Authenticating()
    {
        while (AuthenticationState == AuthenticationState.Authenticating || AuthenticationState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthenticationState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthenticationState = AuthenticationState.Authenticating;
        IAuthenticationService authService = AuthenticationService.Instance;
        for (int tries = 0; tries < maxRetries; tries++)
        {
            try
            {
                if (authService.IsSignedIn && authService.IsAuthorized)
                {
                    AuthenticationState = AuthenticationState.Authenticated;
                    return;
                }
            }
            catch (Exception ex) when (ex is AuthenticationException || ex is RequestFailedException)
            {
                Debug.LogError(ex);
                AuthenticationState = AuthenticationState.Error;
            }
            await authService.SignInAnonymouslyAsync();
            await Task.Delay(1000);
        }
        if (AuthenticationState != AuthenticationState.Authenticating)
        {
            Debug.LogWarning($"Player was not signed in successfully after {maxRetries} attempts");
            AuthenticationState = AuthenticationState.TimeOut;
        }
    }

}


public enum AuthenticationState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}