'use client';

import { GetAlbumCreditsResponse } from "@/api/musicCollections/getAlbumCredits";
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs";
import getMusicCollectionSongsPublic, { MusicCollectionSongResponse } from "@/api/musicCollections/getMusicCollectionSongsPublic";
import addStream from "@/api/songs/addStream";
import getAudioPresignedUrl from "@/api/songs/getAudioPresignedUrl";
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
    contentType: string,
    presignedUrl?: string
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
    const [refreshIntervalId, setRefreshIntervalId] = useState<NodeJS.Timer>();
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

    const updateQueue = (newSongs: ISongModel[], songPublicId?: string) => {
        play(false);
        setCurrentSong(undefined);
        setSongs([]);
        if (!validateUser()) {
            return;
        }
        setSongs(newSongs);
        if (newSongs.length > 0) {
            let songToPlay = newSongs[0];
            if (songPublicId) {
                const existingSong = newSongs.filter(x => x.song.songPublicId.toUpperCase() === songPublicId.toUpperCase())[0];
                if (existingSong) {
                    songToPlay = existingSong;
                }
            }
            setCurrentSong(songToPlay);
            setIsPlaying(true);
        } else {
            play(false);
        }
    };

    const play = (isPlayingNew: boolean) => {
        if (isPlaying === isPlayingNew || !currentSong?.presignedUrl || (isPlayingNew && document.hidden && (!user?.activeSubscriptions || !user.activeSubscriptions.length))) {
            return;
        }

        setIsPlaying(isPlayingNew);
    };

    const next = () => {
        const currentIndex = songs.findIndex(x => x.song.songPublicId.toUpperCase() === currentSong?.song.songPublicId.toUpperCase());
        if (currentIndex === -1) {
            //get next bunch of recommended songs
            return;
        }
        if (currentIndex === songs.length - 1) {
            setCurrentSong(songs[0]);
        } else {
            setCurrentSong(songs[currentIndex + 1]);
        }
    };

    const prev = () => {
        const currentIndex = songs.findIndex(x => x.song.songPublicId.toUpperCase() === currentSong?.song.songPublicId.toUpperCase());
        if (currentIndex === -1 || currentIndex === 0) {
            return;
        }
        setCurrentSong(songs[currentIndex - 1]);
    };

    const fetchAndPlay = useCallback(async (musicSetPublicId: string, songPublicId?: string) => {
        if (!isReady || !validateUser()) {
            return;
        }

        if (lastMusicSetPublicId.toUpperCase() === musicSetPublicId.toUpperCase()) {
            if (songPublicId) {
                const existingSong = songs.filter(x => x.song.songPublicId.toUpperCase() === songPublicId.toUpperCase())[0];
                if (existingSong && existingSong.song.songPublicId.toUpperCase() !== currentSong?.song.songPublicId.toUpperCase()) {
                    setCurrentSong(existingSong);
                }

            } else {
                const firstSong = songs[0];
                if (firstSong && firstSong.song.songPublicId.toUpperCase() !== currentSong?.song.songPublicId.toUpperCase()) {
                    setCurrentSong(firstSong);
                }
            }
            play(true);
            return;
        }

        const audioType = user?.activeSubscriptions && user.activeSubscriptions.length ? "audio/flac" : "audio/mpeg";
        const response = await getMusicCollectionSongs(musicSetPublicId, audioType, axiosPrivate, auth.userId!);
        if (response.ok) {
            setLastMusicSetPublicId(musicSetPublicId);
            updateQueue(response.data!.map(s => ({ song: s, credits: [], contentType: audioType })), songPublicId);
        }
    }, [isReady, songs, play, setCurrentSong, user?.activeSubscriptions, auth.userId, validateUser, lastMusicSetPublicId, setLastMusicSetPublicId]);

    useEffect(() => {
        if (isReady && isAuthenticated && currentSong?.song.songPublicId && auth.userId) {
            if (isPlaying) {
                if (intervalId) {
                    clearInterval(intervalId);
                }

                const id = setInterval(async () => {
                    const response = await addStream(axiosPrivate, auth.userId!, currentSong.song.songPublicId, STREAM_IN_SECONDS);
                    if (!response.ok) {
                        clearInterval(id);
                        setIntervalId(undefined);
                    }
                }, STREAM_IN_SECONDS * 1000); //every 15 sec
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
    }, [isPlaying, currentSong?.song.songPublicId, isReady, isAuthenticated, auth.userId, axiosPrivate]);

    const getUrl = useCallback(async () => {
        const response = await getAudioPresignedUrl(axiosPrivate, auth.userId!, currentSong!.song.songPublicId, currentSong!.contentType);
        if (response.ok) {
            setCurrentSong(prev => prev ? { ...prev, presignedUrl: response.data!.url } : undefined);
        }
        return response.ok;
    }, [currentSong?.song.songPublicId, auth.userId, axiosPrivate]);

    useEffect(() => {
        if (currentSong?.song.songPublicId && isReady && isAuthenticated) {
            if (refreshIntervalId) {
                clearInterval(refreshIntervalId);
            }
            const id = setInterval(async () => {
                const response = await getUrl();
                if (!response) {
                    if (isPlaying) {
                        play(false);
                    }
                    clearInterval(id);
                    setRefreshIntervalId(undefined);
                }
            }, 20 * 10 * 1000); // every 20 min
            setRefreshIntervalId(id);
        } else {
            if (refreshIntervalId) {
                clearInterval(refreshIntervalId);
                setRefreshIntervalId(undefined);
            }
        }

        return () => {
            if (refreshIntervalId) {
                clearInterval(refreshIntervalId);
            }
        };
    }, [currentSong?.song.songPublicId, isReady, isAuthenticated, getUrl]);

    useEffect(() => {
        if (currentSong?.song.songPublicId && isReady && isAuthenticated) {
            getUrl();
        }
    }, [currentSong?.song.songPublicId, isReady, isAuthenticated, getUrl]);

    useEffect(() => {
        const handleVisibilityChange = () => {
            if (document.hidden && isPlaying) {
                play(false);
                toast({
                    title: "Playback paused",
                    description: "Upgrade to keep listening with the app hidden",
                })
            }
        };

        const handleBeforeUnload = () => {
            if (isPlaying) {
                play(false);
            }
        };

        if (user?.activeSubscriptions && user.activeSubscriptions.length > 0) {
            document.removeEventListener('visibilitychange', handleVisibilityChange);
            window.removeEventListener('beforeunload', handleBeforeUnload);
            return;
        }

        document.addEventListener('visibilitychange', handleVisibilityChange);
        window.addEventListener('beforeunload', handleBeforeUnload);

        return () => {
            document.removeEventListener('visibilitychange', handleVisibilityChange);
            window.removeEventListener('beforeunload', handleBeforeUnload);
        };
    }, [user?.activeSubscriptions, isPlaying]);

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

