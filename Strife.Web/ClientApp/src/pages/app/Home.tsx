import * as React from "react"
import { Flex } from "@chakra-ui/react"

import { GuildsSidebar } from "../../components/app/GuildsSidebar";
import { CreateGuildModal } from "../../components/app/CreateGuildModal"

export const Home = () => {
    return (
        <Flex w="100%" h="100%">
            <CreateGuildModal/>
            <GuildsSidebar/>
        </Flex>
    )
}
