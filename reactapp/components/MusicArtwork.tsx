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
import { EllipsisVerticalIcon, PlayCircleIcon, PlayIcon, PlusCircleIcon, User } from "lucide-react"
import useSong from "@/hooks/useSong";
import getMusicCollectionSongs from "@/api/musicCollections/getMusicCollectionSongs";
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu";
import useUser from "@/hooks/useUser";
import { useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

interface Props extends React.HTMLAttributes<HTMLDivElement> {
  aspectRatio?: "portrait" | "square",
  coverPath: string,
  title: string,
  publicId: string;
}

export function MusicArtwork({
  aspectRatio = "square",
  className,
  coverPath,
  title,
  publicId,
  ...props
}: Props) {
  const { updateQueue } = useSong();
  const router = useRouter();
  const { user } = useUser();

  const onPlay = useCallback(async () => {
    const audioType = user?.activeSubscriptions?.length ? "audio/flac" : "audio/mpeg";
    const response = await getMusicCollectionSongs(publicId, audioType);
    if (response.ok) {
      const songs = response.data?.songs ?? [];
      updateQueue(songs.map(s => ({
        id: s.songPublicId,
        title: s.title,
        contentLength: s.contentLength,
        contentType: audioType,
        durationInSeconds: s.durationInSeconds,
        coverPath: coverPath
      })));
    }
  }, [updateQueue, user?.activeSubscriptions?.length]);

  return (
    <div className={cn("space-y-3", className)} {...props}>
      <ContextMenu>
        <ContextMenuTrigger>
          <div className="overflow-hidden rounded-md cursor-pointer">
            <Card className="border-none">
              <CardContent className="flex aspect-square items-center justify-center group relative">
                <Image
                  src={coverPath}
                  alt={title}
                  fill
                  className={cn(
                    "object-cover transition-all group-hover:scale-105",
                    aspectRatio === "portrait" ? "aspect-[3/4]" : "aspect-square"
                  )}
                />
                <div onClick={() => router.push(`/collections/${publicId}`)} className="absolute bg-black rounded-md bg-opacity-0 group-hover:bg-opacity-60 w-full h-full top-0 flex items-end group-hover:opacity-100 transition flex-col justify-between p-2.5">
                  {/* <button onClick={(e) => {
                e.stopPropagation();
                console.log("LIKE")}
                } 
                className="hover:scale-110 text-white opacity-0 transform translate-y-3 group-hover:translate-y-0 group-hover:opacity-100 transition">
              <PlusCircleIcon/>
            </button> */}


                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <button className="hover:scale-125 text-white opacity-0 transform translate-y-3 group-hover:translate-y-0 group-hover:opacity-100 transition">
                        <EllipsisVerticalIcon size={25} />
                      </button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                      <DropdownMenuGroup>
                        <DropdownMenuItem>
                          <User className="mr-2 h-4 w-4" />
                          <span>Your profile</span>
                        </DropdownMenuItem>
                      </DropdownMenuGroup>
                    </DropdownMenuContent>
                  </DropdownMenu>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      console.log("PLAY");
                      onPlay();
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
          <ContextMenuItem>Add to Library</ContextMenuItem>
          <ContextMenuSub>
            <ContextMenuSubTrigger>Add to Playlist</ContextMenuSubTrigger>
            <ContextMenuSubContent className="w-48">
              <ContextMenuItem>
                {/* <PlusCircledIcon className="mr-2 h-4 w-4" /> */}
                New Playlist
              </ContextMenuItem>
              <ContextMenuSeparator />
              {/* {playlists.map((playlist) => (
                <ContextMenuItem key={playlist}>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    stroke="currentColor"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    className="mr-2 h-4 w-4"
                    viewBox="0 0 24 24"
                  >
                    <path d="M21 15V6M18.5 18a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5ZM12 12H3M16 6H3M12 18H3" />
                  </svg>
                  {playlist}
                </ContextMenuItem>
              ))} */}
            </ContextMenuSubContent>
          </ContextMenuSub>
          <ContextMenuSeparator />
          <ContextMenuItem>Play Next</ContextMenuItem>
          <ContextMenuItem>Play Later</ContextMenuItem>
          <ContextMenuItem>Create Station</ContextMenuItem>
          <ContextMenuSeparator />
          <ContextMenuItem>Like</ContextMenuItem>
          <ContextMenuItem>Share</ContextMenuItem>
        </ContextMenuContent>
      </ContextMenu>
      <div className="space-y-1 text-sm">
        <h3 className="mt-2 text-sm font-medium">{title}</h3>
        <p className="text-sm text-muted-foreground">Album • Blaze</p>
      </div>
    </div>
  )
}