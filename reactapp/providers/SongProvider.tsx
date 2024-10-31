'use client';

import addStream from "@/api/songs/addStream";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useUser from "@/hooks/useUser";
import { ReactNode, createContext, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface ISongModel {
    id: string;
    title: string;
    contentLength: number;
    contentType: string;
    durationInSeconds: number;
    coverPath: string;
}

export interface ISongContext {
    currentSong: ISongModel | undefined;
    updateQueue: (newSongs: ISongModel[]) => void;
    queue: ISongModel[];
    next: () => void;
    prev: () => void;
    play: (isPlaying: boolean) => void;
    isPlaying: boolean;
}

const SongContext = createContext<ISongContext | undefined>(undefined);

const STREAM_IN_SECONDS = 15;

const SongContextProvider = ({ children }: Props) => {
    const { auth, isAuthenticated } = useAuth();
    const { user } = useUser();
    const { isReady, axiosPrivate } = useAxiosPrivate();
    const [queue, setQueue] = useState<ISongModel[]>([]);
    const [currentSong, setCurrentSong] = useState<ISongModel>();
    const [isPlaying, setIsPlaying] = useState(false);
    const [intervalId, setIntervalId] = useState<NodeJS.Timer>();

    const updateQueue = (newSongs: ISongModel[]) => {
        setIsPlaying(false);
        setCurrentSong(undefined);
        setQueue([]);
        setQueue(newSongs);
        if (newSongs.length > 0) {
            setCurrentSong(newSongs[0]);
        }
    };

    const play = (isPlaying: boolean) => {
        setIsPlaying(isPlaying);
    };

    const next = () => {
        const currentIndex = queue.findIndex(x => x.id === currentSong?.id);
        if (currentIndex === -1 || currentIndex === queue.length - 1) {
            //get next bunch of recommended songs
            return;
        }
        setCurrentSong(queue[currentIndex + 1]);
    };

    const prev = () => {
        const currentIndex = queue.findIndex(x => x.id === currentSong?.id);
        if (currentIndex === -1 || currentIndex === 0) {
            return;
        }
        setCurrentSong(queue[currentIndex - 1]);
    };

    useEffect(() => {
        if (!isReady || !isAuthenticated) {
            return;
        }

        if (isPlaying) {
            if (intervalId) {
                clearInterval(intervalId);
            }

            const id = setInterval(async () => await addStream(axiosPrivate, auth.userId!, currentSong?.id!, STREAM_IN_SECONDS), STREAM_IN_SECONDS * 1000); // call API every 15 seconds
            setIntervalId(id);
        } else if (intervalId) {
            clearInterval(intervalId); // stop the interval when paused
            setIntervalId(undefined);
        }

        return () => {
            if (intervalId) {
                clearInterval(intervalId);
            }
        };
    }, [isPlaying, currentSong?.id, isReady, isAuthenticated]);

    return (
        <SongContext.Provider value={{ currentSong, updateQueue, next, prev, play, isPlaying, queue }}>
            {children}
        </SongContext.Provider>
    )
}

export const SongProvider: React.FC<Props> = ({ children }) => {
    return <SongContextProvider>{children}</SongContextProvider>;
};

export default SongContext;