import { types } from "mobx-state-tree";

import { Status } from "../status/Status";
import Channel, { ChannelInstance } from "./Channel";

const ChannelStore = types.model({
    channels: types.array(Channel),

    channelSidebarStats: types.optional(Status, "empty")
}).actions((self) => ({
    add: function(channel: ChannelInstance) {
        self.channels.push(channel);
    }
}));

export default ChannelStore