import * as React from "react"
import {LandingNavBar} from "../components/landing/LandingNavBar";

import {Flex} from "@chakra-ui/react"

export function Landing() {
    return (
      <Flex direction={"column"}>
          <LandingNavBar/>
      </Flex>
    )
}
