import * as React from "react";
import { useRef, useState } from "react";

import { Flex } from "@chakra-ui/react";

import * as signalR from "@microsoft/signalr";

import { hostnames } from "../../api/http/base";
import GuildsSidebar from "../../components/app/Guild/GuildsSidebar";
import GuildBanner from "../../components/app/Guild/GuildBanner";
import ChannelsSidebar from "../../components/app/Channel/ChannelsSidebar";
import MessagesList from "../../components/app/Message/MessagesList";

import authorizationService from "../../oidc/AuthorizationService";

import { SignalRHubContext } from "../../signalr/SignalRHubContext";

const Home = (): React.ReactElement => {
    const [selectedGuild, setSelectedGuild] = useState<Nullable<string>>(null);
    const [selectedChannel, setSelectedChannel] = useState<Nullable<string>>(null);
    
    return (
        <SignalRHubContext.Provider value={{
            connection: new signalR.HubConnectionBuilder()
                .withUrl(`${hostnames.api}/hub`, {
                    accessTokenFactory: async () => await authorizationService.getAccessToken(),
                })
                .build(),
            started: useRef(false),
        }}>
            <Flex w="100%" h="100%">
                <GuildsSidebar selectedGuild={selectedGuild} onChangeSelectedGuild={setSelectedGuild}/>
                {selectedGuild && (
                    <>
                        <Flex direction={"column"} w={80}>
                            <GuildBanner selectedGuild={selectedGuild}/>
                            <ChannelsSidebar selectedGuild={selectedGuild} selectedChannel={selectedChannel}
                                onChangeSelectedChannel={channel => setSelectedChannel(channel)}/>
                        </Flex>
                        {selectedChannel &&
                            <MessagesList selectedGuild={selectedGuild} selectedChannel={selectedChannel}/>}
                    </>
                )}
            </Flex>
        </SignalRHubContext.Provider>
    );
}
;

export default Home;
