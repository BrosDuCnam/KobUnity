using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Utils.Network
{
    public class AuthHandler
    {
        public InitializationOptions GenerateAuthenticationOptions(string profile)
        {
            try
            {
                var unityAuthenticationInitOptions = new InitializationOptions();
                if (profile.Length > 0)
                {
                    unityAuthenticationInitOptions.SetProfile(profile);
                }

                return unityAuthenticationInitOptions;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            return null;
        }
        
        public async Task InitializeAndSignInAsync(InitializationOptions initializationOptions)
        {
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public async Task SwitchProfileAndReSignInAsync(string profile)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }

            AuthenticationService.Instance.SwitchProfile(profile);

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public async Task<bool> EnsurePlayerIsAuthorized()
        {
            if (AuthenticationService.Instance.IsAuthorized)
            {
                return true;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (AuthenticationException e)
            {
                Debug.LogError(e);
                
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            return false;
        }
    }
}