import ky from "ky";
import authorizationService from "../../oidc/AuthorizationService"

export const hostnames = {
    api: process.env.STRIFE_API_URL ?? "https://localhost:4001",
    auth: process.env.STRIFE_AUTH_URL ?? "https://localhost:3001",
}

export const apiClient = ky.create({
    prefixUrl: hostnames.api,
    hooks: {
        beforeRequest: [
            async (request) => {
                request.headers.set("Authorization", `Bearer ${await authorizationService.getAccessToken()}`)
            }
        ]
    }
})

export const authClient = ky.create({
    prefixUrl: hostnames.auth,
})