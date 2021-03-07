import ky from "ky";
import authorizationService from "../../oidc/AuthorizationService";

export const hostnames = {
    api: process.env.STRIFE_API_URL ?? "https://localhost:4001",
    auth: process.env.STRIFE_AUTH_URL ?? "https://localhost:3001",
};

export const apiClient = ky.create({
    prefixUrl: hostnames.api,
    hooks: {
        beforeRequest: [
            async (request) => {
                request.headers.set(
                    "Authorization",
                    `Bearer ${await authorizationService.getAccessToken()}`
                );
            },
        ],
        afterResponse: [
            async (request, _options, response) => {
                if (response.status !== 401) return;

                await authorizationService.signIn({ state: {} });
                return ky(request);
            },
        ],
    },
});

export const authClient = ky.create({
    prefixUrl: hostnames.auth,
});
