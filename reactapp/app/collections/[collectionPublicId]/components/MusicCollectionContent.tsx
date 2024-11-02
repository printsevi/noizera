"use client"

import { useCallback, useEffect, useState } from "react"
import Image from "next/image"
import { PlayCircle, MoreVertical, Share, ListPlus, Pause, Heart, PauseCircle, Forward } from "lucide-react"
import { Button } from "@/components/ui/button"
import { ScrollArea } from "@/components/ui/scroll-area"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import getMusicCollectionPublic from "@/api/musicCollections/getMusicCollectionPublic"
import useAuth from "@/hooks/useAuth"
import useAxiosPrivate from "@/hooks/useAxiosPrivate"
import getMusicCollection from "@/api/musicCollections/getMusicCollection"
import useUser from "@/hooks/useUser"
import { ISongModel } from "@/providers/SongProvider"
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs"
import { formatDurationDisplay, getURL } from "@/libs/helpers"
import useSong from "@/hooks/useSong"
import useSignUpModal from "@/hooks/useSignUpModal"

interface Track {
  number: number
  title: string
  duration: string
  liked?: boolean
}

interface AlbumViewProps {
  coverUrl?: string
  title?: string
  artist?: string
  year?: number
  tracks?: Track[]
}

const defaultTracks: Track[] = [
  { number: 1, title: "Lavender Haze", duration: "3:22" },
  { number: 2, title: "Maroon", duration: "3:38" },
  { number: 3, title: "Anti-Hero", duration: "3:20" },
  { number: 4, title: "Snow On The Beach", duration: "4:16" },
  { number: 5, title: "You're On Your Own, Kid", duration: "3:14" },
]

const fetchCollection = async (publicId: string)
  : Promise<MusicCollection | null> => {
  const collectionData = await getMusicCollectionPublic(publicId);
  if (!collectionData.ok) {
    return null;
  }

  return { ...collectionData.data! };
};

interface MusicCollection {
  title: string;
  collectionType: string;
  releaseDate?: string;
  description?: string;
}

interface Props {
  collectionPublicId: string,
}

export default function MusicCollectionContent(props: Props) {
  const signUpModal = useSignUpModal();
  const { auth, isAuthenticated } = useAuth();
  const { user } = useUser();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [musicCollection, setMusicCollection] = useState<MusicCollection>();
  const [songs, setSongs] = useState<ISongModel[]>([]);
  const { updateQueue, isPlaying, play } = useSong();

  const fetchCollection = useCallback(async () => {
    const collectionData = isAuthenticated
      ? await getMusicCollection(props.collectionPublicId, axiosPrivate, auth.userId!)
      : await getMusicCollectionPublic(props.collectionPublicId);
    if (collectionData.ok) {
      setMusicCollection({ ...collectionData.data! });
    }
  }, [isAuthenticated, axiosPrivate, auth.userId]);

  const fetchSongs = useCallback(async () => {
    const audioType = user?.activeSubscriptions?.length ? "audio/flac" : "audio/mpeg";
    const songsData = isAuthenticated
      ? await getMusicCollectionSongs(props.collectionPublicId, audioType)
      : await getMusicCollectionSongs(props.collectionPublicId, audioType);
    if (songsData.ok) {
      setSongs(songsData.data!.songs!.map(s => ({
        id: s.songPublicId,
        title: s.title,
        contentLength: s.contentLength,
        contentType: audioType,
        durationInSeconds: s.durationInSeconds,
        coverPath: ""
      })));
    }
  }, [isAuthenticated, isReady, axiosPrivate, auth.userId, user?.activeSubscriptions?.length]);

  useEffect(() => {
    if (isReady) {
      fetchSongs();
    }
  }, [isReady, isAuthenticated, fetchSongs]);

  useEffect(() => {
    if (isReady) {
      fetchCollection();
    }
  }, [isReady, isAuthenticated, fetchCollection]);

  const trackCount = songs?.length ?? 0
  const [currentTrack, setCurrentTrack] = useState<Track | null>(null)
  const [likedTracks, setLikedTracks] = useState<Set<number>>(new Set())

  const onPlay = useCallback(async () => {
    if (!isReady) {
      return;
    }
    if (!isAuthenticated) {
      signUpModal.onOpen();
    } else {
      updateQueue(songs.map(s => ({
        id: s.id,
        title: s.title,
        contentLength: s.contentLength,
        contentType: s.contentType,
        durationInSeconds: s.durationInSeconds,
        coverPath: ""
      })));
      play(true);
    }
  }, [songs, updateQueue, play, signUpModal, isReady, isAuthenticated]);

  const handleLikeTrack = (trackNumber: number) => {
    setLikedTracks((prev) => {
      const newLiked = new Set(prev)
      if (newLiked.has(trackNumber)) {
        newLiked.delete(trackNumber)
      } else {
        newLiked.add(trackNumber)
      }
      return newLiked
    })
  }

  if (!musicCollection) {
    return <></>
  }

  return (
    <div className="flex flex-col min-h-screen bg-background text-foreground">
      <div className="flex flex-col md:flex-row gap-8 p-10 flex-grow">
        <div className="flex flex-col items-center md:items-start gap-4">
          <Image
            src={`${getURL()}api/music-collections/${props.collectionPublicId}/cover-image`}
            alt={`${musicCollection.title} by ${musicCollection.title}`}
            width={300}
            height={300}
            className="rounded-lg shadow-lg"
          />
          <div className="text-center md:text-left">
            <div className="flex items-center gap-2">
              <h1 className="text-3xl font-bold">{musicCollection.title}</h1>
            </div>
            <p className="text-xl text-muted-foreground">{musicCollection.title}</p>
            <p className="text-sm text-muted-foreground">{musicCollection.releaseDate} • {trackCount} songs</p>
          </div>
          <div className="flex gap-4">
            <Button
              variant="ghost"
              size="icon"
              className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
            >
              <ListPlus className="h-5 w-5" />
            </Button>
            <Button
              disabled={!songs.length}
              variant="ghost"
              size="icon"
              className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
              onClick={(e) => {
                e.stopPropagation();
                onPlay();
              }}
            >
              <PlayCircle className="h-12 w-12" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
            >
              <Forward className="h-5 w-5" />
            </Button>
          </div>
        </div>
        <div className="space-y-1 flex-grow">
          {songs!.map((track, index) => (
            <div
              key={index + 1}
              className="flex items-center gap-4 p-2 rounded-md group relative"
            >
              <div className="absolute inset-y-0 left-0 flex items-center justify-center w-12 opacity-0 group-hover:opacity-100 transition-opacity">
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-colors"
                //onClick={() => handleTrackPlay(track)}
                >
                  <PlayCircle className="h-6 w-6" />
                  <span className="sr-only">Play</span>
                </Button>
              </div>
              <span className="w-12 text-center text-muted-foreground group-hover:opacity-0 transition-opacity">
                {index + 1}
              </span>
              <span className="flex-grow truncate">{track.title}</span>
              <div className="flex items-center gap-2 ml-auto">
                <Button
                  variant="ghost"
                  size="icon"
                  className="opacity-0 group-hover:opacity-100 h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-all"
                //onClick={() => handleLikeTrack(track.number)}
                >
                  <Heart className={`h-5 w-5 ${likedTracks.has(0) ? 'fill-current text-red-500' : ''} transition-colors`} />
                  <span className="sr-only">Like</span>
                </Button>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon" className="opacity-0 group-hover:opacity-100 h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-all">
                      <MoreVertical className="h-5 w-5" />
                      <span className="sr-only">More options</span>
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent>
                    <DropdownMenuItem>
                      <Forward className="mr-2 h-4 w-4" />
                      <span>Share</span>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <ListPlus className="mr-2 h-4 w-4" />
                      <span>Save to playlist</span>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
                <span className="text-muted-foreground w-12 text-right">{formatDurationDisplay(track.durationInSeconds)}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}