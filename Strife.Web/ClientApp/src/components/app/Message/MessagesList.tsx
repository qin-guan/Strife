import * as React from "react";
import { ReactElement, useEffect, useState } from "react";

import { FixedSizeList as List } from "react-window";

import { useMessagesMeta } from "../../../api/swr/messages";

import MessagePage from "./MessagePage";

export interface MessagesListProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const MessagesList = (props: MessagesListProps): Nullable<ReactElement> => {
    const { selectedGuild, selectedChannel } = props;
    const { data: meta, error } = useMessagesMeta(selectedGuild, selectedChannel);
    const [loading, setLoading] = useState(false);
    const [pageCount, setPageCount] = useState(2);

    useEffect(() => {
        setLoading(true);
        const timeout = setTimeout(() => {
            setLoading(false);
        }, 1000);
        return () => {
            clearTimeout(timeout);
        };
    }, [pageCount]);

    if (!meta) return null;

    return (
        <List
            width={1000}
            itemSize={720}
            itemCount={pageCount}
            height={window.innerHeight}
            onScroll={({ scrollOffset }) => {
                if (scrollOffset === 0 && pageCount < meta.Count / meta.PageSize && !loading) {
                    setPageCount(p => p + 1);
                }
            }}
        >
            {({ index, style }) => (
                <MessagePage style={style} selectedGuild={selectedGuild}
                    selectedChannel={selectedChannel} page={pageCount - index - 1}/>
            )}
        </List>
    );
};

export default MessagesList;