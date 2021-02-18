import axios from "axios";

export const useHostnames = {
  api: process.env.STRIFE_API_URL ?? "https://localhost:4001",
  auth: process.env.STRIFE_AUTH_URL ?? "https://localhost:3001",
}

export const apiClient = axios.create({
  baseURL: useHostnames.api,
})

export const authClient = axios.create({
  baseURL: useHostnames.auth,
})
