import * as React from "react"
import { Box, Text, Flex, Spacer, Button, Heading } from "@chakra-ui/react"

function LandingNavBar() {
    return (
        <Flex p={"3"} align="center">
            <Box>
                <Heading size={"md"}>Strife</Heading>
            </Box>
            <Spacer />
            <Box>
                <Button mr={4}>Log in</Button>
                <Button colorScheme={"teal"}>
                    Sign Up
                </Button>
            </Box>
        </Flex>
    )
}

export default React.memo(LandingNavBar)