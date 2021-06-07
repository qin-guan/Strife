/** @jsxImportSource @emotion/react */

import * as React from "react";
import { ReactElement, useState } from "react";

import { Avatar, Box, Center, SkeletonCircle, Spinner, useColorModeValue, useToast } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";

import { css } from "@emotion/react";

import guildsApi from "../../../api/http/guilds";
import { useGuilds } from "../../../api/swr/guilds";
import { Guild } from "../../../models/Guild";

import CreateGuildModal from "./CreateGuildModal";
import { useSignalRHub, SignalRHubMethods } from "../../../signalr/useSignalRHub";
import { HubConnectionState } from "@microsoft/signalr";

export interface GuildsSidebarProps {
    selectedGuild: Nullable<string>;
    onSubscribedToGuild: (guildId: string) => void;
}

const GuildsSidebar = (props: GuildsSidebarProps): Nullable<ReactElement> => {
    const { selectedGuild, onSubscribedToGuild } = props;
    const { data: guilds, error, mutate } = useGuilds();
    const { connectionState, connectionId } = useSignalRHub(SignalRHubMethods.Guild.Created, () => mutate());

    const [createGuildModalOpen, setCreateGuildModalOpen] = useState(false);

    const bg = useColorModeValue("gray.200", "gray.900");
    const borderColor = useColorModeValue("gray.700", "gray.400");

    const toast = useToast();

    if (error) {
        toast({
            title: "Unknown exception",
            description: "An unknown exception occurred while fetching your servers",
            status: "error",
            isClosable: true,
        });
        return null;
    }

    if (!connectionId || connectionState !== HubConnectionState.Connected) {
        return <Spinner/>;
    }
    
    const openCreateGuildModal = (): void => {
        setCreateGuildModalOpen(true);
    };

    const closeCreateGuildModal = (): void => {
        setCreateGuildModalOpen(false);
    };

    return (
        <>
            <CreateGuildModal
                isOpen={createGuildModalOpen}
                onClose={closeCreateGuildModal}
            />
            <Box bg={bg} px={2} css={css`
              overflow-y: scroll;
              -ms-overflow-style: none;
              scrollbar-width: none;

              &::-webkit-scrollbar {
                display: none;
              }
            `}>
                {guilds ? guilds.map((guild: Guild, idx: number) => (
                    <Box py={2} key={idx.toString()}>
                        <Avatar
                            size="md"
                            name={guild.Name}
                            cursor={"pointer"}
                            borderColor={borderColor}
                            showBorder={selectedGuild === guild.Id}

                            onClick={async () => {
                                await guildsApi.subscribe(guild.Id, connectionId);
                                onSubscribedToGuild(guild.Id);
                            }}
                        />
                    </Box>
                )) : (
                    <SkeletonCircle/>
                )}
                <Box py={2}>
                    <Avatar
                        size="md"
                        icon={<AddIcon/>}
                        style={{ cursor: "pointer" }}

                        onClick={openCreateGuildModal}
                    />
                </Box>
            </Box>
        </>
    );
};


export default GuildsSidebar;
