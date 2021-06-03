import * as React from "react";
import useSWR from "swr";

import { fetcher } from "./fetcher";
import { User } from "../../models/User";

export const useUser = (userId: string) => {
    return useSWR<User>(`Users/${userId}`, fetcher, { revalidateOnFocus: false });
};