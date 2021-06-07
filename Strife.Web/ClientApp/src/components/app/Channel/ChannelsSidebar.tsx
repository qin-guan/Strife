import * as React from "react";
import { useMemo, useState } from "react";

import { Box, Flex, Heading, HStack, IconButton, Spinner, Tag, Text, useToast } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";

import { css } from "@emotion/react";
import { Channel } from "../../../models/Channel";

import CreateChannelModal from "./CreateChannelModal";
import { useChannels } from "../../../api/swr/channels";
import { SignalRHubMethods, useSignalRHub } from "../../../signalr/useSignalRHub";
import { HubConnectionState } from "@microsoft/signalr";

export interface ChannelsSidebarProps {
    selectedGuild: string;
    selectedChannel: Nullable<string>;
    onChangeSelectedChannel: (channelId: string) => void;
}

const ChannelsSidebar = (props: ChannelsSidebarProps): Nullable<React.ReactElement> => {
    const { selectedGuild, selectedChannel, onChangeSelectedChannel } = props;
    const { data: channels, error, mutate } = useChannels(selectedGuild);

    const { connectionState, connectionId } = useSignalRHub(SignalRHubMethods.Channel.Created, () => mutate());

    const [createChannelModalOpen, setCreateChannelModalOpen] = useState(false);

    const toast = useToast();

    const groupedChannels = useMemo(() => {
        if (!channels || error) return {};

        return channels.reduce((acc: Record<string, Channel[]>, val: Channel) => {
            if (!acc[val.GroupName]) {
                acc[val.GroupName] = [];
            }
            acc[val.GroupName].push(val);
            return acc;
        }, {});
    }, [channels, error]);

    if (error) {
        toast({
            title: "Unknown exception",
            description: "An unknown exception occurred while fetching your channels",
            status: "error",
            isClosable: true,
        });
        return null;
    }

    if (!connectionId || connectionState !== HubConnectionState.Connected) {
        return (
            <Spinner/>
        );
    }

    const openCreateChannelModal = () => setCreateChannelModalOpen(true);
    const closeCreateChannelModal = () => setCreateChannelModalOpen(false);

    return (
        <>
            <CreateChannelModal isOpen={createChannelModalOpen} onClose={closeCreateChannelModal}
                guildId={selectedGuild}/>
            <Flex p={4} direction={"column"} style={{ height: 1, flex: 1 }}>
                <Box style={{ flex: 1 }} overflow={"scroll"} css={css`
              -ms-overflow-style: none;
              scrollbar-width: none;

              &::-webkit-scrollbar {
                display: none;
              }
            `}>
                    {Object.keys(groupedChannels).map((groupName, idx) => (
                        <Box key={idx.toString()}>
                            <Tag textTransform={"uppercase"} colorScheme={"green"}>{groupName}</Tag>
                            <Box pl={2} my={3}>
                                {groupedChannels[groupName].map((channel, idx2) => (
                                    <HStack
                                        key={idx2.toString()}

                                        py={2}
                                        px={3}
                                        mt={1}
                                        transition={"0.25s"}
                                        backgroundColor={channel.Id === selectedChannel ? "teal" : "gray.800"}
                                        borderRadius={"xl"}
                                        cursor={"pointer"}
                                        onClick={() => onChangeSelectedChannel(channel.Id)}
                                    >
                                        <Heading size={"md"} opacity={0.80}>#</Heading>
                                        <Text opacity={0.75}>{channel.Name}</Text>
                                    </HStack>
                                ))}
                            </Box>
                        </Box>
                    ))}
                </Box>
                <Box mt={6}>
                    <IconButton onClick={openCreateChannelModal} aria-label={"Create channel"} icon={<AddIcon/>}/>
                </Box>
            </Flex>
        </>
    );
};

export default ChannelsSidebar;
