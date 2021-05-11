import { createSlice, PayloadAction, createAsyncThunk } from "@reduxjs/toolkit";
import { Channel } from "./Channel";
import { Guild } from "../guild/Guild";

interface ChannelState {
    entities: Record<string, Channel[]>;
}

const initialState: ChannelState = {
    entities: {},
};

export const channelSlice = createSlice({
    name: "channel",
    initialState,
    reducers: {
        addToGuild: (state, action: PayloadAction<{ channel: Channel; guildId: string }>) => {
            state.entities[action.payload.guildId].push(action.payload.channel);
        },
        setToGuild: (state, action: PayloadAction<{channels: Channel[]; guildId: string}>) => {
            state.entities[action.payload.guildId] = action.payload.channels;
        },
    },
});

export const guildChannels = (state: ChannelState, guildId?: string): Channel[] => guildId ? state.entities[guildId] ?? [] : [];

export const { addToGuild, setToGuild } = channelSlice.actions;

export default channelSlice.reducer;