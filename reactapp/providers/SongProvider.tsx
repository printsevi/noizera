'use client';

import { GetAlbumCreditsResponse } from "@/api/musicCollections/getAlbumCredits";
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs";
import getMusicCollectionSongsPublic, { MusicCollectionSongResponse } from "@/api/musicCollections/getMusicCollectionSongsPublic";
import addStream from "@/api/songs/addStream";
import { toast } from "@/hooks/use-toast";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useSignUpModal from "@/hooks/useSignUpModal";
import useUser from "@/hooks/useUser";
import { ReactNode, createContext, useCallback, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface ISongModel {
    song: MusicCollectionSongResponse,
    credits: GetAlbumCreditsResponse[],
    contentType: string
}

export interface ISongContext {
    currentSong: ISongModel | undefined;
    fetchAndPlay: (musicSetPublicId: string, songPublicId?: string) => Promise<void>;
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
    const [songs, setSongs] = useState<ISongModel[]>([]);
    const [currentSong, setCurrentSong] = useState<ISongModel>();
    const [isPlaying, setIsPlaying] = useState(false);
    const [intervalId, setIntervalId] = useState<NodeJS.Timer>();
    const [limitExceeded, setLimitExceeded] = useState(false);
    const [lastMusicSetPublicId, setLastMusicSetPublicId] = useState("");
    const signUpModal = useSignUpModal();

    const validateUser = useCallback(() => {
        if (!isAuthenticated) {
            signUpModal.onOpen();
            return false;
        }

        if (limitExceeded) {
            toast({
                title: "Your free listening limit has been reached.",
                description: "Upgrade to a subscription to enjoy unlimited access!",
            })
            return false;
        }

        return true;
    }, [isAuthenticated, limitExceeded]);

    const fetchAndPlay = useCallback(async (musicSetPublicId: string, songPublicId?: string) => {
        if (!isReady || !validateUser()) {
            return;
        }

        if (lastMusicSetPublicId === musicSetPublicId) {
            updateQueue(songs, songPublicId);
            return;
        }

        const audioType = user?.activeSubscriptions && user?.activeSubscriptions.length ? "audio/flac" : "audio/mpeg";
        const response = await getMusicCollectionSongs(musicSetPublicId, audioType, axiosPrivate, auth.userId!);
        if (response.ok) {
            setLastMusicSetPublicId(musicSetPublicId);
            updateQueue(response.data!.map(s => ({ song: s, credits: [], contentType: audioType })), songPublicId);
        }
    }, [isReady, songs, user?.activeSubscriptions, auth.userId, validateUser, lastMusicSetPublicId, setLastMusicSetPublicId, axiosPrivate]);

    const updateQueue = (newSongs: ISongModel[], songPublicId?: string) => {
        setIsPlaying(false);
        setCurrentSong(undefined);
        setSongs([]);
        if (!validateUser()) {
            return;
        }
        setSongs(newSongs);
        if (newSongs.length > 0) {
            let songToPlay = newSongs[0];
            if (songPublicId) {
                const existingSong = newSongs.filter(x => x.song.songPublicId === songPublicId)[0];
                if (existingSong) {
                    songToPlay = existingSong;
                }
            }
            setCurrentSong(songToPlay);
            if (!isPlaying) {
                play(true);
            }
        } else {
            play(false);
        }
    };

    const play = (isPlayingNew: boolean) => {
        if (isPlayingNew) {
        }
        setIsPlaying(isPlayingNew);
    };

    const next = () => {
        const currentIndex = songs.findIndex(x => x.song.songPublicId === currentSong?.song.songPublicId);
        if (currentIndex === -1 || currentIndex === songs.length - 1) {
            //get next bunch of recommended songs
            return;
        }
        setCurrentSong(songs[currentIndex + 1]);
    };

    const prev = () => {
        const currentIndex = songs.findIndex(x => x.song.songPublicId === currentSong?.song.songPublicId);
        if (currentIndex === -1 || currentIndex === 0) {
            return;
        }
        setCurrentSong(songs[currentIndex - 1]);
    };

    useEffect(() => {
        if (isReady && isAuthenticated && currentSong?.song.songPublicId) {
            if (isPlaying) {
                if (intervalId) {
                    clearInterval(intervalId);
                }

                const id = setInterval(async () => await addStream(axiosPrivate, auth.userId!, currentSong.song.songPublicId, STREAM_IN_SECONDS), STREAM_IN_SECONDS * 1000); // call API every 15 seconds
                setIntervalId(id);
            } else {
                if (intervalId) {
                    clearInterval(intervalId);
                    setIntervalId(undefined);
                }
            }
        } else {
            if (intervalId) {
                clearInterval(intervalId);
                setIntervalId(undefined);
            }
        }

        return () => {
            if (intervalId) {
                clearInterval(intervalId);
            }
        };
    }, [isPlaying, currentSong?.song.songPublicId, isReady, isAuthenticated]);

    useEffect(() => {
        const handleVisibilityChange = () => {
            if (document.hidden && isPlaying) {
                play(false);
            }
        };

        if (user?.activeSubscriptions && user.activeSubscriptions.length > 0) {
            document.removeEventListener('visibilitychange', handleVisibilityChange);
            return;
        }

        document.addEventListener('visibilitychange', handleVisibilityChange);

        return () => {
            document.removeEventListener('visibilitychange', handleVisibilityChange);
        };
    }, [user, isPlaying]);

    useEffect(() => {
        if (user && user.activeSubscriptions.length === 0 && (user.dayLimitExceeded || user.monthLimitExceeded || user.weekLimitExceeded || user.semiAnnualLimitExceeded)) {
            setLimitExceeded(true);
        }
    }, [user]);

    return (
        <SongContext.Provider value={{ fetchAndPlay, currentSong, next, prev, play, isPlaying, queue: songs }}>
            {children}
        </SongContext.Provider>
    )
}

export const SongProvider: React.FC<Props> = ({ children }) => {
    return <SongContextProvider>{children}</SongContextProvider>;
};

export default SongContext;