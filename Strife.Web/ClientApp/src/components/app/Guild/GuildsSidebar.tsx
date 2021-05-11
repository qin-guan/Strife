/** @jsxImportSource @emotion/react */

import * as React from "react";
import { useCallback, useEffect, useState } from "react";

import { Box, Avatar, useColorModeValue, useToast, Flex } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";

import { css, jsx } from "@emotion/react";

import guildsApi from "../../../api/http/guilds";
import CreateGuildModal from "./CreateGuildModal";
import { useAppSelector } from "../../../store/hooks/useAppSelector";
import { useAppDispatch } from "../../../store/hooks/useAppDispatch";
import { currentGuild, set, subscribeToGuild, updateCurrent } from "../../../models/guild/GuildSlice";
import { Guild } from "../../../models/guild/Guild";

const GuildsSidebar = () => {
    const guildSlice = useAppSelector(s => s.guild);
    const { entities: guilds } = guildSlice;
    const current = currentGuild(guildSlice);
    const dispatch = useAppDispatch();

    const [createGuildModalOpen, setCreateGuildModalOpen] = useState(false);

    const bg = useColorModeValue("gray.200", "gray.900");
    const borderColor = useColorModeValue("gray.700", "gray.400");

    const toast = useToast();

    const fetchGuilds = useCallback(async () => {
        try {
            const guilds = await guildsApi.get();
            dispatch(set(guilds));
        } catch {
            toast({
                title: "Unknown exception",
                description: "An unknown exception occurred while fetching your servers",
                status: "error",
                isClosable: true,
            });
        }
    }, [dispatch, toast]);

    useEffect(() => {
        fetchGuilds();
    }, [fetchGuilds]);

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
                {guilds.map((guild: Guild, idx: number) => (
                    <Box py={2} key={idx.toString()}>
                        <Avatar
                            size="md"
                            name={guild.Name}
                            cursor={"pointer"}
                            borderColor={borderColor}
                            showBorder={current && guild.Id === current.Id}

                            onClick={() => {
                                dispatch(subscribeToGuild(guild.Id));
                                dispatch(updateCurrent(guild));
                            }}
                        />
                    </Box>
                ))}
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
