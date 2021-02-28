import * as React from "react"
import { Flex } from "@chakra-ui/react"

import { GuildSidebar } from "../../components/app/GuildSidebar";

export const Home = () => {
    return (
        <Flex w="100%" h="100%">
            <GuildSidebar/>
        </Flex>
    )
}
