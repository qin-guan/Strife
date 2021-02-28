import { authClient } from "./Base";

export const client = {
    get: async ({ clientConfigPath }: {clientConfigPath: string}) => {
        return await authClient.get(clientConfigPath)
    }
}