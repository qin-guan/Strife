import { WebStorageStateStore } from "oidc-client";
import { authClient } from "./Base";

export interface OidcConfigResponse {
    automaticSilentRenew?: boolean;
    includeIdTokenInSilentRenew?: boolean;
    userStore?: WebStorageStateStore;
}

export const client = {
    get: async ({ clientConfigPath }: {clientConfigPath: string}) => {
        return await authClient.get(clientConfigPath).json<OidcConfigResponse>()
    }
}