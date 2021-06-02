import * as React from "react";
import { CSSProperties, memo } from "react";
import { Message as MessageModel } from "../../../models/Message";
import { useMessages } from "../../../api/swr/messages";

export interface MessagePageProps {
    selectedGuild: string;
    selectedChannel: string;
    style: CSSProperties;
    page: number;
}

const MessagePage = (props: MessagePageProps) => {
    const { selectedGuild, selectedChannel, page, style } = props;
    const { data , error } = useMessages(selectedGuild, selectedChannel, page);
    
    if (!data) {
        return <>Loading...</>;
    }
    
    return (
        <div style={style}>
            {[...data.reverse()].map((message, idx) => (
                <>
                    {message ? <div key={idx.toString()}>{message.Content}</div> : <div style={{ height: 24, width: 100, backgroundColor: "gray" }}/>}
                </>
            ))}
        </div>
    );
};

export default memo(MessagePage);