import { User, UserManager, WebStorageStateStore, Profile } from "oidc-client";
import { OidcPaths, ApplicationName } from "./AuthorizationConstants";
import { client as oidcClient } from "../api/http/Oidc";

export class AuthorizeService {
    _callbacks: { callback: () => void, subscription: number }[] = [];
    _nextSubscriptionId = 0;
    _user?: User;
    _isAuthenticated = false;

    _userManager?: UserManager;

    // By default pop ups are disabled because they don't work properly on Edge.
    // If you want to enable pop up authentication simply set this flag to false.
    _popUpDisabled = true;

    async isAuthenticated(): Promise<boolean> {
        const user = await this.getUser();
        return !!user;
    }

    async getUser(): Promise<Nullable<Profile>> {
        if (this._user && this._user.profile) {
            return this._user.profile;
        }

        const { userManager } = await this.getUserManager()
        const user = await userManager.getUser();
        return user && user.profile;
    }

    async getAccessToken(): Promise<Nullable<string>> {
        const { userManager } = await this.getUserManager()
        const user = await userManager.getUser();
        return user && user.access_token;
    }

    // We try to authenticate the user in three different ways:
    // 1) We try to see if we can authenticate the user silently. This happens
    //    when the user is already logged in on the IdP and is done using a hidden iframe
    //    on the client.
    // 2) We try to authenticate the user using a PopUp Window. This might fail if there is a
    //    Pop-Up blocker or the user has disabled PopUps.
    // 3) If the two methods above fail, we redirect the browser to the IdP to perform a traditional
    //    redirect flow.
    async signIn({ state }: {state: any}) {
        const { userManager } = await this.getUserManager()

        try {
            const silentUser = await userManager.signinSilent(this.createArguments({}));
            this.updateState({ user: silentUser });

            return this.success({ state })
        } catch (silentError) {
            // User might not be authenticated, fallback to popup authentication
            console.log("Silent authentication error: ", silentError);

            try {
                if (this._popUpDisabled) {
                    throw new Error("Popup disabled. Change 'AuthorizeService.js:AuthorizeService._popupDisabled' to false to enable it.")
                }

                const popUpUser = await userManager.signinPopup(this.createArguments({}));
                this.updateState({ user: popUpUser });

                return this.success({ state })
            } catch (popUpError) {
                if (popUpError.message === "Popup window closed") {
                    // The user explicitly cancelled the login action by closing an opened popup.

                    return this.fail({ message: "The user closed the window." })
                } else if (!this._popUpDisabled) {
                    console.log("Popup authentication error: ", popUpError);
                }

                // PopUps might be blocked by the user, fallback to redirect
                try {
                    await userManager.signinRedirect(this.createArguments(state));

                    return this.redirect()
                } catch (redirectError) {
                    console.log("Redirect authentication error: ", redirectError);

                    return this.fail({ message: redirectError })
                }
            }
        }
    }

    async completeSignIn({ url }: { url: string }) {
        try {
            const { userManager } = await this.getUserManager()
            const user = await userManager.signinCallback(url);
            this.updateState({ user });

            return this.success(user)
        } catch (error) {
            console.log("There was an error signing in: ", error);
            return this.fail({ message: "There was an error signing in." })
        }
    }

    // We try to sign out the user in two different ways:
    // 1) We try to do a sign-out using a PopUp Window. This might fail if there is a
    //    Pop-Up blocker or the user has disabled PopUps.
    // 2) If the method above fails, we redirect the browser to the IdP to perform a traditional
    //    post logout redirect flow.
    async signOut({ state }: {state: object}) {
        const { userManager } = await this.getUserManager()

        try {
            if (this._popUpDisabled) {
                throw new Error("Popup disabled. Change 'AuthorizeService.js:AuthorizeService._popupDisabled' to false to enable it.")
            }

            await userManager.signoutPopup(this.createArguments({}));
            this.updateState({});

            return this.success({ state })
        } catch (popupSignOutError) {
            console.log("Popup signout error: ", popupSignOutError);
            try {
                await userManager.signoutRedirect(this.createArguments(state));

                return this.redirect()
            } catch (redirectSignOutError) {
                console.log("Redirect signout error: ", redirectSignOutError);

                return this.fail({ message: redirectSignOutError })
            }
        }
    }

    async completeSignOut({ url }: { url: string }) {
        const { userManager } = await this.getUserManager()
        try {
            const response = await userManager.signoutCallback(url);
            this.updateState({});

            // @ts-ignore (data doesn't seem to be included in SignoutResponse)
            return this.success({ state: response.data })
        } catch (error) {
            console.log(`There was an error trying to log out '${error}'.`);
            return this.fail({ message: error })
        }
    }

    success({ state }: { state: any }) {
        return { status: AuthenticationResultStatus.Success, state }
    }

    fail({ message }: { message: string }) {
        return { status: AuthenticationResultStatus.Fail, message }
    }

    redirect() {
        return { status: AuthenticationResultStatus.Redirect }
    }

    updateState({ user }: { user?: User }) {
        this._user = user;
        this._isAuthenticated = !!this._user;
        this.notifySubscribers();
    }

    subscribe({ callback }: {callback: () => void}) {
        this._callbacks.push({ callback, subscription: this._nextSubscriptionId++ });
        return this._nextSubscriptionId - 1;
    }

    unsubscribe({ subscriptionId }: {subscriptionId: number}) {
        this._callbacks = this._callbacks.filter(cb => cb.subscription !== subscriptionId)
    }

    notifySubscribers() {
        for (let i = 0; i < this._callbacks.length; i++) {
            const callback = this._callbacks[i].callback;
            callback();
        }
    }

    createArguments({ data }: {data?: object}): { useReplaceToNavigate: boolean, data?: object } {
        return { useReplaceToNavigate: true, data };
    }

    async getUserManager(): Promise<{ userManager: UserManager }> {
        await this.ensureUserManagerInitialized();
        return { userManager: this._userManager! };
    }

    async ensureUserManagerInitialized() {
        if (this._userManager) {
            return;
        }

        try {
            const data = await oidcClient.get({ clientConfigPath: OidcPaths.ApiAuthorizationClientConfigurationUrl })

            data.automaticSilentRenew = true;
            data.includeIdTokenInSilentRenew = true;
            data.userStore = new WebStorageStateStore({
                prefix: ApplicationName
            });

            this._userManager = new UserManager(data);

            this._userManager.events.addUserSignedOut(async () => {
                await this._userManager?.removeUser();
                this.updateState({});
            });
        } catch {
            throw new Error(`Could not load settings for ${ApplicationName}`)
        }
    }

    static get instance() {
        return authService
    }
}

const authService = new AuthorizeService();

export default authService;

export const AuthenticationResultStatus = {
    Redirect: "redirect",
    Success: "success",
    Fail: "fail"
};
