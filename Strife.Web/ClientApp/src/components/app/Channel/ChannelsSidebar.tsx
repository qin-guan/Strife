import * as React from "react";
import { useCallback, useEffect, useMemo, useState } from "react";

import {
    Box,
    Button, Flex,
    Heading,
    HStack, IconButton,
    Tag,
    Text,
    useToast
} from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";

import channelsApi from "../../../api/http/channels";
import messagesApi from "../../../api/http/messages";

import CreateChannelModal from "./CreateChannelModal";

import { guildChannels, setToGuild } from "../../../models/channel/ChannelSlice";

import { useAppDispatch } from "../../../store/hooks/useAppDispatch";
import { Channel } from "../../../models/channel/Channel";
import { useAppSelector } from "../../../store/hooks/useAppSelector";
import { css } from "@emotion/react";

const ChannelsSidebar = (): Nullable<React.ReactElement> => {
    const [createChannelModalOpen, setCreateChannelModalOpen] = useState(false);
    const guildId = useAppSelector(s => s.guild.selectedEntity);
    const channelSlice = useAppSelector(s => s.channel);
    const channels = guildChannels(channelSlice, guildId);

    const dispatch = useAppDispatch();

    const [selectedChannelId, setSelectedChannelId] = useState("");

    const toast = useToast();

    const fetchChannels = useCallback(async () => {
        if (!guildId) return;

        try {
            const { get } = channelsApi(guildId);
            const channels = await get();
            dispatch(setToGuild({ channels, guildId }));
        } catch (e) {
            console.error(e);
            toast({
                title: "Unknown exception",
                description: "An unknown exception occurred while fetching the server channels",
                status: "error",
                isClosable: true,
            });
        }
    }, [guildId, toast, dispatch]);

    useEffect(() => {
        if (!guildId) return;
        fetchChannels();
    }, [guildId, fetchChannels]);

    useEffect(() => {
        if (!selectedChannelId) return;
        const { get } = messagesApi(guildId, selectedChannelId);
        get();
    }, [selectedChannelId, guildId]);

    const groupedChannels = useMemo(() => {
        if (!guildId) return {};
        return channels.reduce((acc: Record<string, Channel[]>, val: Channel) => {
            if (!acc[val.GroupName]) {
                acc[val.GroupName] = [];
            }
            acc[val.GroupName].push(val);
            return acc;
        }, {});
    }, [channels, guildId]);

    if (!guildId) return null;

    const openCreateChannelModal = () => setCreateChannelModalOpen(true);
    const closeCreateChannelModal = () => setCreateChannelModalOpen(false);

    return (
        <>
            <CreateChannelModal isOpen={createChannelModalOpen} onClose={closeCreateChannelModal} guildId={guildId}/>
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
                                        backgroundColor={channel && channel.Id === selectedChannelId ? "teal" : "gray.800"}
                                        borderRadius={"xl"}
                                        cursor={"pointer"}
                                        onClick={() => setSelectedChannelId(channel.Id)}
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
