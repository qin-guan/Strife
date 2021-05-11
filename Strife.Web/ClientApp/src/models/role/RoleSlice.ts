import { createSlice, PayloadAction, createAsyncThunk } from "@reduxjs/toolkit";
import { Role } from "./Role";

interface RoleState {
    entities: Role[];
}

const initialState: RoleState = {
    entities: [],
};

export const roleSlice = createSlice({
    name: "role",
    initialState,
    reducers: {
        add: (state, action: PayloadAction<Role>) => {
            state.entities.push(action.payload);
        },
        set: (state, action: PayloadAction<Role[]>) => {
            state.entities = action.payload;
        },
    },
});

export const { add, set } = roleSlice.actions;

export default roleSlice.reducer;