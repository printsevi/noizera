'use client';

import Image from "next/image"
//import { PlusCircledIcon } from "@radix-ui/react-icons"

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
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs";
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu";
import useUser from "@/hooks/useUser";
import { useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getCoverImageSrc, getURL } from "@/libs/helpers";
import useLibrary from "@/hooks/useLibrary";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useAuth from "@/hooks/useAuth";
import addSavedMusicCollection from "@/api/savedMusicCollections/addSavedMusicCollection";
import { CollectionType } from "@/api/common";
import useSignUpModal from "@/hooks/useSignUpModal";
import deleteSavedMusicCollection from "@/api/savedMusicCollections/deleteSavedMusicCollection";
import Link from "next/link";
import { toast } from "@/hooks/use-toast";

interface Props {
  aspectRatio?: "portrait" | "square",
  title: string,
  publicId: string;
  isSaved: boolean;
  collectionType: CollectionType,
  ownerName: string,
  ownerUsername: string,
  songCount: number
}

export function MusicArtwork({
  aspectRatio = "square",
  collectionType,
  title,
  publicId,
  ownerName,
  ownerUsername,
  songCount,
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
      const domain = window.location.origin;
      const urlToCopy = `${domain}/collections/${publicId.toLowerCase()}`;
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
            isSaved: true
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
                  src={getCoverImageSrc(publicId)}
                  alt={title}
                  fill
                  sizes="500"
                  priority={false}
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
        {isAuthenticated && <ContextMenuContent className="w-40">
          <ContextMenuItem>
            <CopyPlus className="mr-2 h-4 w-4" />
            <span>Save to library</span>
          </ContextMenuItem>
          <ContextMenuItem>
            <Forward className="mr-2 h-4 w-4" />
            <span>Share</span>
          </ContextMenuItem>
        </ContextMenuContent>}
      </ContextMenu>
      <div className="space-y-1 text-sm">
        <h3 className="mt-2 text-sm font-medium truncate"><Link href={`/collections/${publicId.toLowerCase()}`} key={`/collections/${publicId}`} className="hover:underline">{title}</Link></h3>
        <p className="text-sm text-muted-foreground line-clamp-2 text-ellipsis">{songCount} songs • <Link href={`/profiles/${ownerUsername.toLowerCase()}`} key={`/profiles/${ownerUsername.toLowerCase()}`} className="hover:underline">{ownerName}</Link></p>
      </div>
    </div>
  )
}