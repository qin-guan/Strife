import * as React from "react";
import {
    Modal,
    ModalOverlay,
    ModalHeader,
    ModalContent,
    ModalBody,
    ModalFooter,
    ModalCloseButton,
    FormControl,
    FormLabel,
    Input,
    Button, useToast,
} from "@chakra-ui/react";

import guildsApi from "../../../api/http/guilds";
import { useState } from "react";

const guildNameRegex = /^[a-z0-9]+$/i;

export interface CreateGuildModalProps {
    isOpen: boolean;
    onClose: () => void;
}

const CreateGuildModal = (props: CreateGuildModalProps): React.ReactElement => {
    const { isOpen, onClose } = props;

    const [guildName, setGuildName] = useState("");
    const [creatingGuild, setCreatingGuild] = useState(false);

    const toast = useToast();

    const initialRef = React.useRef(null);

    const onGuildCreate = async () => {
        setCreatingGuild(true);

        try {
            await guildsApi.create({
                Name: guildName,
            });

            setGuildName("");
            onClose();
        } catch {
            toast({
                title: "Unknown exception",
                description: "An unknown exception occurred while fetching your servers",
                status: "error",
                isClosable: true,
            });
        } finally {
            setCreatingGuild(false);
        }
    };

    const onGuildNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setGuildName(event.currentTarget.value);
    };

    return (
        <Modal
            initialFocusRef={initialRef}
            isOpen={isOpen}
            onClose={onClose}
            isCentered
        >
            <ModalOverlay/>
            <ModalContent>
                <ModalHeader>Create Server</ModalHeader>
                <ModalCloseButton/>
                <ModalBody pb={6}>
                    <FormControl>
                        <FormLabel>Name</FormLabel>
                        <Input
                            ref={initialRef}
                            placeholder="Name"
                            value={guildName}
                            onChange={onGuildNameChange}
                        />
                    </FormControl>
                </ModalBody>

                <ModalFooter>
                    <Button
                        isLoading={creatingGuild}
                        isDisabled={!guildNameRegex.test(guildName)}
                        colorScheme="blue"
                        mr={3}
                        onClick={onGuildCreate}
                    >
                        Create
                    </Button>
                    <Button onClick={onClose}>Cancel</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
};

export default CreateGuildModal;
