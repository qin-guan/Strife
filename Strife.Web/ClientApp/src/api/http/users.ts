import { User } from "../../models/User";
import { apiClient } from "./base";

const find = async (userId: string): Promise<User> => {
    return await apiClient.get(`Users/${userId}`).json();
};