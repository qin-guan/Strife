import axios from "axios";

export const hostnames = {
    api: process.env.STRIFE_API_URL ?? "https://localhost:4001",
    auth: process.env.STRIFE_AUTH_URL ?? "https://localhost:3001",
}

export const apiClient = axios.create({
    baseURL: hostnames.api,
})

export const authClient = axios.create({
    baseURL: hostnames.auth,
})