import { Instance, types } from "mobx-state-tree";

export const Channel = types.model({
    Id: types.string,
    Name: types.string,
    IsVoice: types.boolean,
});

export type ChannelInstance = Instance<typeof Channel>

export default Channel;