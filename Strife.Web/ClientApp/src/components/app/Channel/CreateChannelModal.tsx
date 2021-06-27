import * as React from "react";
import { ChangeEvent, ReactElement, useState } from "react";

import {
    Button,
    FormControl, FormLabel, Input,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent, ModalFooter,
    ModalHeader,
    ModalOverlay, Switch, useToast, VStack
} from "@chakra-ui/react";

import channelsApi from "../../../api/http/channels";

interface CreateChannelModalProps {
    isOpen: boolean;
    onClose: () => void;

    guildId: string;
}

const channelNameRegex = /^[a-z0-9]+$/i;

const CreateChannelModal = (props: CreateChannelModalProps): ReactElement => {
    const { isOpen, onClose, guildId } = props;

    const [channelName, setChannelName] = useState("");
    const [groupName, setGroupName] = useState("");
    const [isVoice, setIsVoice] = useState(false);

    const [creatingChannel, setCreatingChannel] = useState(false);

    const toast = useToast();

    const initialRef = React.useRef(null);

    const onChannelCreate = async () => {
        setCreatingChannel(true);

        try {
            await channelsApi(guildId).create({
                Name: channelName,
                GroupName: groupName,
                IsVoice: isVoice
            });

            setChannelName("");
            setGroupName("");
            onClose();
        } catch {
            toast({
                title: "Unknown exception",
                description: "An unknown exception occurred while creating your channel",
                status: "error",
                isClosable: true,
            });
        } finally {
            setCreatingChannel(false);
        }
    };

    const onChannelNameChange = (event: ChangeEvent<HTMLInputElement>) => {
        setChannelName(event.currentTarget.value);
    };

    const onGroupNameChange = (event: ChangeEvent<HTMLInputElement>) => {
        setGroupName(event.currentTarget.value);
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
                <ModalHeader>Create channel</ModalHeader>
                <ModalCloseButton/>
                <ModalBody pb={6}>
                    <VStack spacing={"3"}>
                        <FormControl>
                            <FormLabel>Name</FormLabel>
                            <Input
                                ref={initialRef}
                                placeholder="Name"
                                value={channelName}
                                onChange={onChannelNameChange}
                            />
                        </FormControl>
                        <FormControl>
                            <FormLabel>Group</FormLabel>
                            <Input
                                placeholder="Group"
                                value={groupName}
                                onChange={onGroupNameChange}
                            />
                        </FormControl>
                    </VStack>
                </ModalBody>

                <ModalFooter>
                    <Button
                        isLoading={creatingChannel}
                        isDisabled={!channelNameRegex.test(channelName)}
                        colorScheme="blue"
                        mr={3}
                        onClick={onChannelCreate}
                    >
                        Create
                    </Button>
                    <Button onClick={onClose}>Cancel</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
};

export default CreateChannelModal;
