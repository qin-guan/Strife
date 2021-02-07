import axios from "axios";

export const apiClient = axios.create({
  baseURL: process.env.STRIFE_API_URL ?? "https://localhost:4001",
})

export const authClient = axios.create({
  baseURL: process.env.STRIFE_AUTH_URL ?? "https://localhost:3001",
})
