import * as React from "react"
import { observer } from "mobx-react";
import { Flex } from "@chakra-ui/react"

import { GuildsSidebar } from "../../components/app/GuildsSidebar";
import { CreateGuildModal } from "../../components/app/CreateGuildModal"

import guildHubService from "../../signalr/GuildHubService"

import { useMst } from "../../models/root/Root"

export const Home = observer(() => {
    const {
        guildStore: { addGuild }
    } = useMst()

    React.useEffect(() => {
        guildHubService.onGuildCreated((guild) => {
            addGuild({ guild })
        })
    }, [addGuild])
    
    return (
        <Flex w="100%" h="100%">
            <CreateGuildModal/>
            <GuildsSidebar/>
        </Flex>
    )
})
