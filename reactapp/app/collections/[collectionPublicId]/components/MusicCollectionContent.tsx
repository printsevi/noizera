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
import getMusicCollectionPublic, { MusicCollectionResponse } from "@/api/musicCollections/getMusicCollectionPublic"
import useAuth from "@/hooks/useAuth"
import useAxiosPrivate from "@/hooks/useAxiosPrivate"
import getMusicCollection from "@/api/musicCollections/getMusicCollection"
import useUser from "@/hooks/useUser"
import { ISongModel } from "@/providers/SongProvider"
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs"
import { formatDurationDisplay, getURL } from "@/libs/helpers"
import useSong from "@/hooks/useSong"
import useSignUpModal from "@/hooks/useSignUpModal"
import getAlbumCredits, { GetAlbumCreditsResponse } from "@/api/musicCollections/getAlbumCredits"
import Link from "next/link"
import React from "react"

interface Track {
  number: number
  title: string
  duration: string
  liked?: boolean
}


interface Props {
  collectionPublicId: string,
}

export default function MusicCollectionContent(props: Props) {
  const signUpModal = useSignUpModal();
  const { auth, isAuthenticated } = useAuth();
  const { user } = useUser();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [musicCollection, setMusicCollection] = useState<MusicCollectionResponse>();
  const [songs, setSongs] = useState<ISongModel[]>([]);
  const [credits, setCredits] = useState<GetAlbumCreditsResponse[]>([]);
  const { updateQueue, isPlaying, play } = useSong();
  const [hoveredTrack, setHoveredTrack] = useState<string | null>(null)

  const fetchCollection = useCallback(async () => {
    const collectionData = isAuthenticated
      ? await getMusicCollection(props.collectionPublicId, axiosPrivate, auth.userId!)
      : await getMusicCollectionPublic(props.collectionPublicId);
    if (collectionData.ok) {
      setMusicCollection(collectionData.data!);
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
        coverPath: "",
        ownerName: "",
        ownerPublicId: ""
      })));
    }
  }, [isAuthenticated, isReady, axiosPrivate, auth.userId, user?.activeSubscriptions?.length]);

  const fetchAlbumCredits = useCallback(async () => {
    const response = await getAlbumCredits(props.collectionPublicId)
    if (response.ok) {
      setCredits(response.data!);
    }
  }, [isReady]);

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

  useEffect(() => {
    if (isReady) {
      fetchAlbumCredits();
    }
  }, [isReady, fetchAlbumCredits]);

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
        coverPath: "",
        ownerName: "",
        ownerPublicId: ""
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
    //<div className="container mx-auto px-4 py-8">
    <div className="flex flex-col lg:flex-row lg:space-x-8">
      <div className="lg:w-1/3 mb-8 lg:mb-0">
        <div className="flex flex-col items-center lg:items-start min-w-0 w-full">
          <Image
            src={`${getURL()}api/music-collections/${props.collectionPublicId}/cover-image`}
            alt={`${musicCollection.title} by ${musicCollection.title}`}
            width={300}
            height={300}
            className="rounded-lg shadow-lg mb-4"
          />
          <h1 className="text-4xl font-bold mb-4 text-center lg:text-left line-clamp-2 hover:line-clamp-none transition-all duration-300 ease-in-out">
            {musicCollection.title}
          </h1>
          <div className="text-muted-foreground mb-4 text-center lg:text-left w-full">
            <p className="mb-1">{musicCollection.releaseDate} • {trackCount} songs</p>
            <div className="flex flex-wrap">
              <React.Fragment>
                <Link href={`/profiles/${musicCollection.ownerUsername.toLowerCase()}`} className="hover:underline truncate max-w-full">
                  {musicCollection.ownerName}
                </Link>
              </React.Fragment>
              {credits.map((credit, index) => (
                <React.Fragment key={credit.username}>
                  <span>&nbsp;•&nbsp;</span>
                  <Link href={credit.username ? `/profiles/${credit.username.toLowerCase()}` : ""} className="hover:underline truncate max-w-full">{credit.name ?? credit.profileName}</Link>
                </React.Fragment>
              ))}
            </div>
          </div>
          <div className="flex items-center space-x-4 mb-4">
            <Button
              variant="ghost"
              size="icon"
              className="rounded-full"
            >
              <ListPlus className="h-6 w-6" />
            </Button>
            <Button
              disabled={!songs.length}
              variant="ghost"
              size="icon"
              className="rounded-full"
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
              className="rounded-full"
            >
              <Forward className="h-5 w-5" />
            </Button>
          </div>
        </div>
      </div>
      <div className="lg:w-2/3">
        <div className="space-y-2">
          {songs.map((track, index) => (
            <div
              key={track.id}
              className="flex items-center space-x-4 p-2 rounded-md transition-colors duration-200"
              onMouseEnter={() => setHoveredTrack(track.id)}
              onMouseLeave={() => setHoveredTrack(null)}
            >
              <div className="w-8 text-center flex-shrink-0">
                {hoveredTrack === track.id ? (
                  <PlayCircle className="h-6 w-6" />
                ) : (
                  <span className="text-muted-foreground">{index + 1}</span>
                )}
              </div>
              <div className="flex-grow min-w-0 h-8">
                <p className="font-medium truncate">{track.title}</p>
                {/* <p className="text-sm text-gray-500 truncate">{track.ownerName}1</p> */}
              </div>
              <div className="flex items-center space-x-4">
                {hoveredTrack === track.id && (
                  <Button size="icon" variant="ghost" className="h-8 w-8">
                    <Heart className="h-4 w-4" />
                    <span className="sr-only">Like</span>
                  </Button>
                )}
                <div className="text-sm text-muted-foreground flex-shrink-0">{formatDurationDisplay(track.durationInSeconds)}</div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
    //</div>
  )

  // return (
  //   <div className="flex flex-col min-h-screen bg-background text-foreground">
  //     <div className="flex flex-col md:flex-row gap-8 p-10 flex-grow">
  //       <div className="flex flex-col items-center md:items-start gap-4">
  //         <Image
  //           src={`${getURL()}api/music-collections/${props.collectionPublicId}/cover-image`}
  //           alt={`${musicCollection.title} by ${musicCollection.title}`}
  //           width={300}
  //           height={300}
  //           className="rounded-lg shadow-lg"
  //         />
  //         <div className="text-center md:text-left">
  //           <div className="flex items-center gap-2 max-w-[300px]">
  //             <h1 className="text-2xl font-bold line-clamp-2 text-ellipsis">{musicCollection.title}</h1>
  //           </div>
  //           <p className="text-xl text-muted-foreground">
  //             <Link href={`/profiles/${musicCollection.ownerUsername.toLowerCase()}`} key={`/profiles/${musicCollection.ownerUsername.toLowerCase()}`} className="hover:underline">
  //               {musicCollection.ownerName}
  //             </Link>
  //             {credits.map((credit, index) => (
  //               <span key={index}> • {credit.username ? <Link href={`/profiles/${credit.username.toLowerCase()}`} key={`/profiles/${credit.username.toLowerCase()}`} className="hover:underline"> {credit.name}</Link> : credit.profileName}</span>
  //             ))}
  //           </p>
  //           <p className="text-sm text-muted-foreground">{musicCollection.releaseDate} • {trackCount} songs</p>
  //         </div>
  //         <div className="flex gap-4">
  //           <Button
  //             variant="ghost"
  //             size="icon"
  //             className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
  //           >
  //             <ListPlus className="h-5 w-5" />
  //           </Button>
  //           <Button
  //             disabled={!songs.length}
  //             variant="ghost"
  //             size="icon"
  //             className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
  //             onClick={(e) => {
  //               e.stopPropagation();
  //               onPlay();
  //             }}
  //           >
  //             <PlayCircle className="h-12 w-12" />
  //           </Button>
  //           <Button
  //             variant="ghost"
  //             size="icon"
  //             className="rounded-full hover:bg-primary hover:text-primary-foreground transition-colors"
  //           >
  //             <Forward className="h-5 w-5" />
  //           </Button>
  //         </div>
  //       </div>
  //       <div className="space-y-1 flex-grow">
  //         {songs!.map((track, index) => (
  //           <div
  //             key={index + 1}
  //             className="flex items-center gap-4 p-2 rounded-md group relative"
  //           >
  //             <div className="absolute inset-y-0 left-0 flex items-center justify-center w-12 opacity-0 group-hover:opacity-100 transition-opacity">
  //               <Button
  //                 variant="ghost"
  //                 size="icon"
  //                 className="h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-colors"
  //               //onClick={() => handleTrackPlay(track)}
  //               >
  //                 <PlayCircle className="h-6 w-6" />
  //                 <span className="sr-only">Play</span>
  //               </Button>
  //             </div>
  //             <span className="w-12 text-center text-muted-foreground group-hover:opacity-0 transition-opacity">
  //               {index + 1}
  //             </span>
  //             <span className="flex-grow truncate max-w-[200px]">{track.title}</span>
  //             <div className="flex items-center gap-2 ml-auto">
  //               <Button
  //                 variant="ghost"
  //                 size="icon"
  //                 className="opacity-0 group-hover:opacity-100 h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-all"
  //               //onClick={() => handleLikeTrack(track.number)}
  //               >
  //                 <Heart className={`h-5 w-5 ${likedTracks.has(0) ? 'fill-current text-red-500' : ''} transition-colors`} />
  //                 <span className="sr-only">Like</span>
  //               </Button>
  //               {/* <DropdownMenu>
  //                 <DropdownMenuTrigger asChild>
  //                   <Button variant="ghost" size="icon" className="opacity-0 group-hover:opacity-100 h-10 w-10 p-0 hover:bg-primary hover:text-primary-foreground transition-all">
  //                     <MoreVertical className="h-5 w-5" />
  //                     <span className="sr-only">More options</span>
  //                   </Button>
  //                 </DropdownMenuTrigger>
  //                 <DropdownMenuContent>
  //                   <DropdownMenuItem>
  //                     <Forward className="mr-2 h-4 w-4" />
  //                     <span>Share</span>
  //                   </DropdownMenuItem>
  //                   <DropdownMenuItem>
  //                     <ListPlus className="mr-2 h-4 w-4" />
  //                     <span>Save to playlist</span>
  //                   </DropdownMenuItem>
  //                 </DropdownMenuContent>
  //               </DropdownMenu> */}
  //               <span className="text-muted-foreground w-12 text-right">{formatDurationDisplay(track.durationInSeconds)}</span>
  //             </div>
  //           </div>
  //         ))}
  //       </div>
  //     </div>
  //   </div>
  // )
}