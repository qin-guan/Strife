import {authClient} from "./Base";

export const settings = {
  get: async (clientConfigPath: string) => {
    return await authClient.get(clientConfigPath)
  }
}
