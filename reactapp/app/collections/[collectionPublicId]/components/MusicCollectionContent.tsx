"use client"

import { useCallback, useEffect, useState } from "react"
import Image from "next/image"
import { PlayCircle, Heart, PauseCircle, Forward, CopyMinus, CopyPlus } from "lucide-react"
import { Button } from "@/components/ui/button"
import getMusicCollectionPublic, { MusicCollectionResponse } from "@/api/musicCollections/getMusicCollectionPublic"
import useAuth from "@/hooks/useAuth"
import useAxiosPrivate from "@/hooks/useAxiosPrivate"
import getMusicCollection from "@/api/musicCollections/getMusicCollection"
import useUser from "@/hooks/useUser"
import getMusicCollectionSongsPublic, { MusicCollectionSongResponse } from "@/api/musicCollections/getMusicCollectionSongsPublic"
import { formatDurationDisplay, getCoverImageSrc, getURL } from "@/libs/helpers"
import useSong from "@/hooks/useSong"
import useSignUpModal from "@/hooks/useSignUpModal"
import getAlbumCredits, { GetAlbumCreditsResponse } from "@/api/musicCollections/getAlbumCredits"
import Link from "next/link"
import React from "react"
import { toast } from "@/hooks/use-toast"
import useLibrary from "@/hooks/useLibrary"
import addSavedMusicCollection from "@/api/savedMusicCollections/addSavedMusicCollection"
import deleteSavedMusicCollection from "@/api/savedMusicCollections/deleteSavedMusicCollection"
import { CollectionType, ProfileType } from "@/api/common"
import addSongToFavourites from "@/api/songs/addSongToFavourites"
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs"
import removeSongFromFavourites from "@/api/songs/removeSongFromFavourites"

interface Props {
  collectionPublicId: string,
}

export default function MusicCollectionContent(props: Props) {
  const { auth, isAuthenticated } = useAuth();
  const { user } = useUser();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [musicCollection, setMusicCollection] = useState<MusicCollectionResponse>();
  const [songs, setSongs] = useState<MusicCollectionSongResponse[]>([]);
  const [credits, setCredits] = useState<GetAlbumCreditsResponse[]>([]);
  const { fetchAndPlay } = useSong();
  const [hoveredTrack, setHoveredTrack] = useState<string | null>(null)
  const [collectionIsSaved, setCollectionIsSaved] = useState(false);
  const { addCollection, removeCollection } = useLibrary();
  const signUpModal = useSignUpModal();

  const fetchCollection = useCallback(async () => {
    const collectionData = isAuthenticated
      ? await getMusicCollection(props.collectionPublicId, axiosPrivate, auth.userId!)
      : await getMusicCollectionPublic(props.collectionPublicId);
    if (collectionData.ok) {
      setMusicCollection(collectionData.data!);
      setCollectionIsSaved(collectionData.data!.isSaved);
    }
  }, [isAuthenticated, axiosPrivate, auth.userId]);

  const fetchSongs = useCallback(async () => {
    const songsData = isAuthenticated
      ? await getMusicCollectionSongs(props.collectionPublicId, "audio/mpeg", axiosPrivate, auth.userId!)
      : await getMusicCollectionSongsPublic(props.collectionPublicId);
    if (songsData.ok) {
      setSongs(songsData.data!);
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

  const trackCount = songs?.length ?? 0;

  const onClickLike = useCallback(async (songPublicId: string, favouriteSongId?: string) => {
    if (!isReady || !isAuthenticated) {
      return;
    }

    const response = favouriteSongId
      ? await removeSongFromFavourites(axiosPrivate, auth.userId!, favouriteSongId)
      : await addSongToFavourites(axiosPrivate, auth.userId!, songPublicId);
    if (response.ok) {
      const updatedSongs = songs.map((songItem) => {
        if (songItem.songPublicId === songPublicId) {
          return { ...songItem, favouriteSongId: response.data?.value };
        }
        return { ...songItem, favouriteSongId: songItem.favouriteSongId };
      });
      setSongs(updatedSongs);
    }

  }, [fetchAndPlay, isReady, isAuthenticated, setSongs, songs, axiosPrivate, auth.userId]);

  const onPlay = useCallback(async (songPublicId?: string) => {
    if (!isReady) {
      return;
    }
    await fetchAndPlay(props.collectionPublicId, songPublicId);
  }, [fetchAndPlay, isReady]);

  const onSaveDeleteToggle = useCallback(async () => {
    if (!isReady || !musicCollection) {
      return;
    }

    if (!isAuthenticated) {
      signUpModal.onOpen();
      return;
    }

    if (!collectionIsSaved) {
      const response = await addSavedMusicCollection(axiosPrivate, auth.userId!, props.collectionPublicId);
      if (response.ok) {
        addCollection({
          publicId: props.collectionPublicId,
          title: musicCollection.title,
          collectionType: musicCollection.collectionType,
          ownerName: musicCollection.ownerName,
          ownerUsername: musicCollection.ownerUsername,
          songCount: trackCount,
          isSaved: true
        });
        setCollectionIsSaved(true);
      }
    } else {
      const response = await deleteSavedMusicCollection(axiosPrivate, auth.userId!, props.collectionPublicId);
      if (response.ok) {
        removeCollection(props.collectionPublicId);
        setCollectionIsSaved(false);
      }
    }
  }, [isAuthenticated, trackCount, musicCollection, isReady, collectionIsSaved, auth.userId, axiosPrivate, addCollection]);

  const copyCollectionUrl = async () => {
    try {
      const urlToCopy = `https://noizera.com/collections/${props.collectionPublicId.toLowerCase()}`;
      await navigator.clipboard.writeText(urlToCopy);
      toast({
        title: "The URL copied to clipboard!"
      });
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Oops... something went wrong",
        description: "Please try again in a while",
      });
    }
  };

  if (!musicCollection) {
    return <></>
  }

  return (
    <div className="flex flex-col lg:flex-row lg:space-x-8">
      <div className="lg:w-1/3 mb-8 lg:mb-0">
        <div className="flex flex-col items-center min-w-0 w-full">
          <Image
            src={musicCollection.collectionType === CollectionType.Album ? getCoverImageSrc(props.collectionPublicId) : "/images/favourites.png"}
            alt={`${musicCollection.title} by ${musicCollection.title}`}
            width={300}
            height={300}
            className="rounded-lg shadow-lg mb-4"
          />
          <h1 className="text-4xl font-bold mb-4 text-center line-clamp-2 hover:line-clamp-none transition-all duration-300 ease-in-out">
            {musicCollection.title}
          </h1>
          <div className="text-muted-foreground mb-4 text-center w-full">
            <p className="mb-1">{musicCollection.releaseDate} • {trackCount} songs</p>
            {credits.length > 0 && <div className="flex flex-wrap justify-center mb-1">
              {musicCollection.ownerProfileType === ProfileType.Artist && credits.length > 0 && <React.Fragment>
                <Link href={`/profiles/${musicCollection.ownerUsername.toLowerCase()}`} className="hover:underline truncate max-w-full">
                  {musicCollection.ownerName}
                </Link>
              </React.Fragment>}
              {credits.map((credit, index) => (
                <React.Fragment key={credit.username}>
                  <span>&nbsp;•&nbsp;</span>
                  {credit.username ? <Link href={`/profiles/${credit.username.toLowerCase()}`} className="hover:underline truncate max-w-full">{credit.name}</Link> : <span className="truncate max-w-full">{credit.profileName}</span>}
                </React.Fragment>
              ))}
            </div>}
            <div className="flex flex-wrap justify-center">
              <React.Fragment>
                by&nbsp;
                <Link href={`/profiles/${musicCollection.ownerUsername.toLowerCase()}`} className="hover:underline truncate max-w-full">
                  {musicCollection.ownerName}
                </Link>
              </React.Fragment>
            </div>
          </div>
          <div className="flex items-center space-x-4 mb-4">
            <Button
              variant="ghost"
              size="icon"
              className="rounded-full"
              onClick={(e: { stopPropagation: () => void; }) => {
                e.stopPropagation();
                onSaveDeleteToggle();
              }}>
              {collectionIsSaved ? <CopyMinus className="h-6 w-6" /> : <CopyPlus className="h-6 w-6" />}
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
              onClick={(e: { stopPropagation: () => void; }) => {
                e.stopPropagation();
                copyCollectionUrl();
              }}
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
              key={track.songPublicId}
              className="h-8 flex items-center space-x-4 p-2 rounded-md transition-colors duration-200"
              onMouseEnter={() => setHoveredTrack(track.songPublicId)}
              onMouseLeave={() => setHoveredTrack(null)}
            >
              <div className="w-8 text-center flex-shrink-0">
                {hoveredTrack === track.songPublicId ? (
                  <Button size="icon" variant="ghost" className="rounded-full" onClick={(e) => {
                    e.stopPropagation();
                    onPlay(track.songPublicId);
                  }}>
                    <PlayCircle className="h-6 w-6" />
                    <span className="sr-only">Play</span>
                  </Button>

                ) : (
                  <span className="text-muted-foreground">{index + 1}</span>
                )}
              </div>
              <div className="flex-grow min-w-0">
                <p className="font-medium truncate">{track.title}</p>
                {/* <p className="text-sm text-gray-500 truncate">{track.ownerName}1</p> */}
              </div>
              <div className="flex items-center space-x-4">
                {hoveredTrack === track.songPublicId && (
                  <Button onClick={() => onClickLike(track.songPublicId, track.favouriteSongId)} size="icon" variant="ghost" className="rounded-full">
                    <Heart className={`h-6 w-6 ${track.favouriteSongId ? "fill-current" : ""}`} />
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
  )
}