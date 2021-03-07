import * as React from "react";
import { observer } from "mobx-react";
import { useMst } from "../../models/root/Root";
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
    Button,
} from "@chakra-ui/react";

const guildNameRegex = /^[a-z0-9]+$/i;

export const CreateGuildModal = observer(() => {
    const {
        guildStore: { createGuildModalOpen, closeCreateGuildModal, createGuild },
    } = useMst();

    const [guildName, setGuildName] = React.useState("");
    const [creatingGuild, setCreatingGuild] = React.useState(false);

    const initialRef = React.useRef(null);

    const onClose = () => {
        closeCreateGuildModal();
    };

    const onGuildCreate = async () => {
        setCreatingGuild(true);

        await createGuild({ Name: guildName, Id: "" });

        setCreatingGuild(false);
        onClose();
    };

    const onGuildNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setGuildName(event.currentTarget.value);
    };

    return (
        <Modal
            initialFocusRef={initialRef}
            isOpen={createGuildModalOpen}
            onClose={onClose}
            isCentered
        >
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>Create Server</ModalHeader>
                <ModalCloseButton />
                <ModalBody pb={6}>
                    <FormControl>
                        <FormLabel>Name</FormLabel>
                        <Input
                            ref={initialRef}
                            placeholder="My friends and I"
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
});
