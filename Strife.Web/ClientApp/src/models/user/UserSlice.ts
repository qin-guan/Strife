import { createSlice, PayloadAction, createAsyncThunk } from "@reduxjs/toolkit";
import { User } from "./User";

interface UserState {
    entities: User[];
}

const initialState: UserState = {
    entities: [],
};

export const userSlice = createSlice({
    name: "user",
    initialState,
    reducers: {
        add: (state, action: PayloadAction<User>) => {
            state.entities.push(action.payload);
        },
        set: (state, action: PayloadAction<User[]>) => {
            state.entities = action.payload;
        },
    },
});

export const { add, set } = userSlice.actions;

export default userSlice.reducer;