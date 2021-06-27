import * as React from "react";
import {
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent, ModalFooter,
    ModalHeader,
    ModalOverlay
} from "@chakra-ui/react";
import { ReactElement } from "react";

export interface GuildSettingsModelProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function GuildSettingsModal(props: GuildSettingsModelProps): ReactElement {
    const { isOpen, onClose } = props;

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            size={"6xl"}
        >
            <ModalOverlay/>
            <ModalContent>
                <ModalHeader>Settings</ModalHeader>
                <ModalCloseButton/>
                <ModalBody pb={6}>
                </ModalBody>

                <ModalFooter>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
}