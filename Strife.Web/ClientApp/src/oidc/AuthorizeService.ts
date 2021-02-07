import {User, UserManager, UserManagerSettings, WebStorageStateStore} from 'oidc-client';
import {ApplicationPaths, ApplicationName} from './ApiAuthorizationConstants';

import {settings} from "../api/OIDC";

export class AuthorizeService {
  private _userManager: UserManager;
  private _user?: User;
  private _isAuthenticated: boolean;

  private _popUpDisabled: boolean = false;

  constructor(settings: UserManagerSettings) {
    if (!settings) {
      throw new Error("AuthorizeService cannot be initialized directly, please use .build()")
    }

    this._isAuthenticated = false;
    this._userManager = new UserManager(settings)

    this._userManager.events.addUserSignedOut(async () => {
      await this._userManager.removeUser();
      this.updateState();
    });
  }

  public static async build() {
    try {
      const {data} = await settings.get(ApplicationPaths.ApiAuthorizationClientConfigurationUrl)

      data.automaticSilentRenew = true;
      data.includeIdTokenInSilentRenew = true;
      data.userStore = new WebStorageStateStore({
        prefix: ApplicationName
      });

      return new AuthorizeService(data)
    } catch {
      console.log("Could not build AuthorizeService")
    }
  }

  public async signIn(state: any) {
    try {
      const silentUser = await this._userManager.signinSilent(this.createArguments());
      this.updateState(silentUser);
      return this.success(state);
    } catch (silentError) {
      // User might not be authenticated, fallback to popup authentication
      console.log("Silent authentication error: ", silentError);

      try {
        if (this._popUpDisabled) {
          throw new Error('Popup disabled. Change \'AuthorizeService.js:AuthorizeService._popupDisabled\' to false to enable it.')
        }

        const popUpUser = await this._userManager.signinPopup(this.createArguments());
        this.updateState(popUpUser);
        return this.success(state);
      } catch (popUpError) {
        if (popUpError.message === "Popup window closed") {
          // The user explicitly cancelled the login action by closing an opened popup.
          return this.error("The user closed the window.");
        } else if (!this._popUpDisabled) {
          console.log("Popup authentication error: ", popUpError);
        }

        // PopUps might be blocked by the user, fallback to redirect
        try {
          await this._userManager.signinRedirect(this.createArguments(state));
          return this.redirect();
        } catch (redirectError) {
          console.log("Redirect authentication error: ", redirectError);
          return this.error(redirectError);
        }
      }
    }
  }

  async completeSignIn(url: string) {
    try {
      const user = await this._userManager.signinCallback(url);
      this.updateState(user);
      return this.success(user && user.state);
    } catch (error) {
      console.log('There was an error signing in: ', error);
      return this.error('There was an error signing in.');
    }
  }

  async signOut(state: any) {
    try {
      if (this._popUpDisabled) {
        throw new Error('Popup disabled. Change \'AuthorizeService.js:AuthorizeService._popupDisabled\' to false to enable it.')
      }

      await this._userManager.signoutPopup(this.createArguments());
      this.updateState();
      return this.success(state);
    } catch (popupSignOutError) {
      console.log("Popup signout error: ", popupSignOutError);
      try {
        await this._userManager.signoutRedirect(this.createArguments(state));
        return this.redirect();
      } catch (redirectSignOutError) {
        console.log("Redirect signout error: ", redirectSignOutError);
        return this.error(redirectSignOutError);
      }
    }
  }

  async completeSignOut(url: string) {
    try {
      const response = await this._userManager.signoutCallback(url);
      this.updateState();
      // @ts-ignore
      return this.success(response && response.data);
    } catch (error) {
      console.log(`There was an error trying to log out '${error}'.`);
      return this.error(error);
    }
  }

  public async isAuthenticated() {
    const user = await this.getUser();
    return !!user;
  }

  public async getUser() {
    if (this._user && this._user.profile) {
      return this._user.profile;
    }

    const user = await this._userManager.getUser();
    return user && user.profile;
  }

  public async getAccessToken() {
    const user = await this._userManager.getUser();
    return user && user.access_token;
  }

  public updateState(user?: User) {
    this._user = user;
    this._isAuthenticated = !!this._user
  }

  createArguments(state?: any) {
    return { useReplaceToNavigate: true, data: state };
  }

  error(message: string) {
    return { status: AuthenticationResultStatus.Fail, message };
  }

  success(state: any) {
    return { status: AuthenticationResultStatus.Success, state };
  }

  redirect() {
    return { status: AuthenticationResultStatus.Redirect };
  }
}

export const AuthenticationResultStatus = {
  Redirect: 'redirect',
  Success: 'success',
  Fail: 'fail'
};
