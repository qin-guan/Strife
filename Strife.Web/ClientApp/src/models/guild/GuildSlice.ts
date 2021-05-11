import { createSlice, PayloadAction, createAsyncThunk } from "@reduxjs/toolkit";
import { Guild } from "./Guild";
import hubService from "../../signalr/HubService";
import { AppDispatch, RootState } from "../../store";

interface GuildState {
    entities: Guild[];
    subscribedEntities: string[];
    selectedEntity: string;
}

const initialState: GuildState = {
    entities: [],
    subscribedEntities: [],
    selectedEntity: ""
};

export const guildSlice = createSlice({
    name: "guild",
    initialState,
    reducers: {
        add: (state, action: PayloadAction<Guild>) => {
            state.entities.push(action.payload);
        },
        set: (state, action: PayloadAction<Guild[]>) => {
            state.entities = action.payload;
        },
        addSubscribed: (state, action: PayloadAction<string>) => {
            if (state.subscribedEntities.includes(action.payload)) return;
            state.subscribedEntities.push(action.payload);
        },
        updateCurrent: (state, action: PayloadAction<Guild>) => {
            state.selectedEntity = action.payload.Id;
        }
    },
});

export const { add, set, addSubscribed, updateCurrent } = guildSlice.actions;

export const subscribeToGuild = createAsyncThunk<void, string, { dispatch: AppDispatch; state: RootState }>("hub/subscribeToGuild", async (guildId, thunkAPI) => {
    const { guild: { subscribedEntities } } = thunkAPI.getState();
    if (subscribedEntities.includes(guildId)) return;
    
    thunkAPI.dispatch(addSubscribed(guildId));
    await hubService.subscribeToGuild(guildId);
});

export const currentGuild = (state: GuildState): Guild | undefined => state.entities.find(e => e.Id === state.selectedEntity);

export default guildSlice.reducer;