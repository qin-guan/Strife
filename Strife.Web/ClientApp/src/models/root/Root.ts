import { useContext, createContext } from "react";
import { types, Instance, onSnapshot } from "mobx-state-tree";

import GuildStore from "../guild/GuildStore";
import ChannelStore from "../channel/ChannelStore";
import UserStore from "../user/UserStore";

const Root = types.model({
    guildStore: GuildStore,
    channelStore: ChannelStore,
    userStore: UserStore
});

let initialState = Root.create({
    guildStore: {
        guilds: [],
    },
    channelStore: {
        channels: []
    },
    userStore: {
        users: []
    }
});

const data = localStorage.getItem("rootState");
if (data) {
    const json = JSON.parse(data);
    if (Root.is(json)) {
        initialState = Root.create(json);
    }
}

export const rootStore = initialState;

onSnapshot(rootStore, snapshot => {
    console.log("Snapshot: ", snapshot);
    localStorage.setItem("rootState", JSON.stringify(snapshot));
});
  
export type RootInstance = Instance<typeof Root>;
const RootStoreContext = createContext<null | RootInstance>(null);
  
export const Provider = RootStoreContext.Provider;

export function useMst(): RootInstance {
    const store = useContext(RootStoreContext);
    if (store === null) {
        throw new Error("Store cannot be null, please add a context provider");
    }
    return store;
}
  