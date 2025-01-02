'use client';

import Image from "next/image"

import { cn } from "@/lib/utils"
import {
  ContextMenu,
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuSeparator,
  ContextMenuSub,
  ContextMenuSubContent,
  ContextMenuSubTrigger,
  ContextMenuTrigger,
} from "@/components/ui/context-menu"
import { Card, CardContent } from "./ui/card"
import { CopyMinus, CopyPlus, EllipsisVerticalIcon, Forward, PlayCircleIcon, PlayIcon, PlusCircleIcon, User } from "lucide-react"
import useSong from "@/hooks/useSong";
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu";
import { useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getCoverImageSrc, getURL } from "@/libs/helpers";
import useLibrary from "@/hooks/useLibrary";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useAuth from "@/hooks/useAuth";
import addSavedMusicCollection from "@/api/savedMusicCollections/addSavedMusicCollection";
import { CollectionType, ProfileType } from "@/api/common";
import deleteSavedMusicCollection from "@/api/savedMusicCollections/deleteSavedMusicCollection";
import Link from "next/link";
import { toast } from "@/hooks/use-toast";
import { GetAlbumCreditsResponse } from "@/api/musicCollections/getAlbumCredits";
import React from "react";

interface Props {
  aspectRatio?: "portrait" | "square",
  title: string,
  publicId: string;
  isSaved: boolean;
  collectionType: CollectionType,
  ownerName: string,
  ownerUsername: string,
  ownerProfileType: ProfileType,
  credits: GetAlbumCreditsResponse[],
  songCount: number,
  releaseDate: string
}

export function MusicArtwork({
  aspectRatio = "square",
  collectionType,
  title,
  publicId,
  ownerName,
  ownerUsername,
  songCount,
  releaseDate,
  ownerProfileType,
  credits,
  isSaved = false
}: Props) {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { fetchAndPlay } = useSong();
  const { addCollection, removeCollection } = useLibrary();
  const router = useRouter();
  const [collectionIsSaved, setCollectionIsSaved] = useState(isSaved);

  const copyCollectionUrl = async () => {
    try {
      const urlToCopy = `https://noizera.com/collections/${publicId.toLowerCase()}`;
      await navigator.clipboard.writeText(urlToCopy);
      toast({
        title: "The URL copied to clipboard!"
      });
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Oops... something went wrong",
        description: error as string,
      });
    }
  };

  const onSaveDeleteToggle = useCallback(async () => {
    if (!isReady) {
      return;
    }
    if (isAuthenticated) {
      if (!collectionIsSaved) {
        const response = await addSavedMusicCollection(axiosPrivate, auth.userId!, publicId);
        if (response.ok) {
          addCollection({
            publicId: publicId,
            title: title,
            collectionType: collectionType,
            ownerName: ownerName,
            ownerUsername: ownerUsername,
            songCount: songCount,
            isSaved: true,
            releaseDate: releaseDate,
            ownerProfileType: ownerProfileType,
            credits: credits
          });
          setCollectionIsSaved(true);
        }
      } else {
        const response = await deleteSavedMusicCollection(axiosPrivate, auth.userId!, publicId);
        if (response.ok) {
          removeCollection(publicId);
          setCollectionIsSaved(false);
        }
      }

    }
  }, [isAuthenticated, isReady, collectionIsSaved, auth.userId, axiosPrivate, addCollection]);

  if (!publicId) {
    return (<></>);
  }

  return (
    <div className="space-y-3">
      <ContextMenu>
        <ContextMenuTrigger>
          <div className="overflow-hidden rounded-md cursor-pointer">
            <Card className="border-none">
              <CardContent className="flex aspect-square items-center justify-center group relative">
                <Image
                  src={collectionType === CollectionType.Album ? getCoverImageSrc(publicId) : "/images/favourites.png"}
                  alt={title}
                  fill
                  sizes="500"
                  priority={false}
                  placeholder="blur"
                  blurDataURL="/images/favourites.png"
                  className={cn(
                    "object-cover transition-all group-hover:scale-105",
                    aspectRatio === "portrait" ? "aspect-[3/4]" : "aspect-square"
                  )}
                />
                <div onClick={() => router.push(`/collections/${publicId.toLowerCase()}`)} className="absolute bg-black rounded-md bg-opacity-0 group-hover:bg-opacity-60 w-full h-full top-0 flex items-end group-hover:opacity-100 transition flex-col justify-between p-2.5">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <button className="hover:scale-125 text-white opacity-0 transform translate-y-3 group-hover:translate-y-0 group-hover:opacity-100 transition">
                        <EllipsisVerticalIcon size={25} />
                      </button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                      <DropdownMenuGroup>
                        {isAuthenticated && <DropdownMenuItem
                          onClick={(e: { stopPropagation: () => void; }) => {
                            e.stopPropagation();
                            onSaveDeleteToggle();
                          }}>
                          {collectionIsSaved ? <CopyMinus className="mr-2 h-4 w-4" /> : <CopyPlus className="mr-2 h-4 w-4" />}
                          <span>{collectionIsSaved ? "Remove from library" : "Save to library"}</span>
                        </DropdownMenuItem>}
                        <DropdownMenuItem onClick={(e: { stopPropagation: () => void; }) => {
                          e.stopPropagation();
                          copyCollectionUrl();
                        }}>
                          <Forward className="mr-2 h-4 w-4" />
                          <span>Share</span>
                        </DropdownMenuItem>
                      </DropdownMenuGroup>
                    </DropdownMenuContent>
                  </DropdownMenu>
                  <button
                    onClick={async (e) => {
                      e.stopPropagation();
                      await fetchAndPlay(publicId);
                    }}
                    className="hover:scale-150 text-white opacity-0 transform translate-y-3 group-hover:translate-y-0 group-hover:opacity-100 transition"
                  >
                    <PlayIcon size={35} />
                  </button>
                </div>
              </CardContent>
            </Card>
          </div>
        </ContextMenuTrigger>
        <ContextMenuContent className="w-40">
          {isAuthenticated && <ContextMenuItem
            onClick={(e: { stopPropagation: () => void; }) => {
              e.stopPropagation();
              onSaveDeleteToggle();
            }}>
            {collectionIsSaved ? <CopyMinus className="mr-2 h-4 w-4" /> : <CopyPlus className="mr-2 h-4 w-4" />}
            <span>{collectionIsSaved ? "Remove from library" : "Save to library"}</span>
          </ContextMenuItem>}
          <ContextMenuItem onClick={(e: { stopPropagation: () => void; }) => {
            e.stopPropagation();
            copyCollectionUrl();
          }}>
            <Forward className="mr-2 h-4 w-4" />
            <span>Share</span>
          </ContextMenuItem>
        </ContextMenuContent>
      </ContextMenu>
      <div className="space-y-1 text-sm">
        <h3 className="mt-2 text-sm font-medium truncate"><Link href={`/collections/${publicId.toLowerCase()}`} key={`/collections/${publicId}`} className="hover:underline">{title}</Link></h3>
        {credits.length > 0 && <div className="flex flex-wrap justify-start text-muted-foreground">
          {ownerProfileType === ProfileType.Artist && credits.length > 0 && <React.Fragment>
            <Link href={`/profiles/${ownerUsername.toLowerCase()}`} className="hover:underline truncate max-w-full">
              {ownerName}
            </Link>
            <span>&nbsp;•&nbsp;</span>
          </React.Fragment>}
          {credits.map((credit, index) => (
            <React.Fragment key={credit.username}>
              {index > 0 && <span>&nbsp;•&nbsp;</span>}
              {credit.username ? <Link href={`/profiles/${credit.username.toLowerCase()}`} className="hover:underline truncate max-w-full">{credit.name}</Link> : <span className="truncate max-w-full">{credit.profileName}</span>}
            </React.Fragment>
          ))}
        </div>}
        <p className="text-sm text-muted-foreground line-clamp-2 text-ellipsis">{songCount} songs • by&nbsp;<Link href={`/profiles/${ownerUsername.toLowerCase()}`} key={`/profiles/${ownerUsername.toLowerCase()}`} className="hover:underline">{ownerName}</Link></p>
      </div>
    </div>
  )
}