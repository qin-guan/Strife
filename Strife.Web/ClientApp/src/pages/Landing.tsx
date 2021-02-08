import * as React from "react"
import {LandingNavBar} from "../components/NavBars/LandingNavBar";

import {Flex} from "@chakra-ui/react"

export function Landing() {
    return (
      <Flex direction={"column"}>
          <LandingNavBar/>
      </Flex>
    )
}
