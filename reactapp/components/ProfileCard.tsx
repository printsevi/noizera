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
import { getCoverImageSrc, getProfileImageSrc, getURL } from "@/libs/helpers";
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
import ImageWithFallback from "./ImageWithFallback";

interface Props {
  name: string,
  publicId: string,
  username: string
}

export function ProfileCard({
  username,
  name,
  publicId
}: Props) {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { fetchAndPlay } = useSong();
  const { addCollection, removeCollection } = useLibrary();
  const router = useRouter();

  if (!publicId) {
    return (<></>);
  }

  return (
    <div>
      <div className="overflow-hidden rounded-full cursor-pointer">
        <Card className="border-none">
          <CardContent className="flex aspect-square items-center justify-center group relative">
            <ImageWithFallback
              src={getProfileImageSrc(publicId)}
              sizes="500"
              priority={false}
              fill
              alt={name}
              fallbackSrc="/images/user.png"
              className={cn(
                "object-cover transition-all group-hover:scale-105 rounded-full"
              )}
            />
            <div onClick={() => router.push(`/profiles/${username.toLowerCase()}`)} className="absolute bg-black rounded-md bg-opacity-0 group-hover:bg-opacity-60 w-full h-full top-0 flex items-end group-hover:opacity-100 transition flex-col justify-between p-2.5">
            </div>
          </CardContent>
        </Card>
      </div>
      <div className="text-sm text-center">
        <h3 className="mt-2 font-medium truncate"><Link href={`/profiles/${username.toLowerCase()}`} key={`/profiles/${username}`} className="hover:underline">{name}</Link></h3>
      </div>
    </div>
  )
}