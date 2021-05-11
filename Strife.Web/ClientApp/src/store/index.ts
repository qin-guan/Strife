import { configureStore } from "@reduxjs/toolkit";

import channelReducer from "../models/channel/ChannelSlice";
import guildReducer from "../models/guild/GuildSlice";
import roleReducer from "../models/role/RoleSlice";
import userReducer from "../models/user/UserSlice";

export const store =  configureStore({
    reducer: {
        channel: channelReducer,
        guild: guildReducer,
        role: roleReducer,
        user: userReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch