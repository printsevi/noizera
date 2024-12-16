'use client';

import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  TouchSensor,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import { SongAccordionItem } from '@/components/SongAccordionItem';
import { InputField } from '@/components/InputField';
import { ComboboxField, ComboboxItemProps } from '@/components/ComboboxField';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import useAuth from '@/hooks/useAuth';
import getOrCreateAlbumDraft, { GetOrCreateAlbumDraftCreditResponse } from '@/api/musicCollections/getOrCreateAlbumDraft';
import useSWR from 'swr';
import Box from '@/components/Box';
import updateMusicCollectionTitle from '@/api/musicCollections/updateMusicCollectionTitle';
import getArtists, { GetArtistResponse } from '@/api/profiles/getArtists';
import addAlbumCredit from '@/api/musicCollections/addAlbumCredit';
import deleteAlbumCredit from '@/api/musicCollections/deleteAlbumCredit';
import { ProfileType } from '@/api/common';
import addAlbumSong from '@/api/musicCollections/addAlbumSong';
import updateAlbumSongTitle from '@/api/musicCollections/updateAlbumSongTitle';
import updateMusicCollectionSongSequence from '@/api/musicCollections/updateMusicCollectionSongSequence';
import uploadAudioFile from '@/api/songs/uploadAudioFile';
import getAntiforgeryToken from '@/api/auth/getAntiforgeryToken';
import deleteAlbumSong from '@/api/musicCollections/deleteAlbumSong';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import submitAlbum from '@/api/musicCollections/submitAlbum';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '@/components/ui/alert-dialog';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import ImageUploader from '@/components/ImageUploader';
import uploadAlbumCoverImage from '@/api/musicCollections/uploadAlbumCoverImage';
import { getProfileImageSrc, getURL } from '@/libs/helpers';
import PurpleButton from '@/components/Button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from '@/hooks/use-toast';
import { useRouter } from 'next/navigation';
import { Skeleton } from '@/components/ui/skeleton';
import useUser from '@/hooks/useUser';
import { CalendarIcon, ExternalLink, Plus, UserPlus, X } from 'lucide-react';
import deleteAudioFile from '@/api/songs/deleteAudioFile';
import deleteAlbumCoverImage from '@/api/musicCollections/deleteAlbumCoverImage';
import AuthRequired from '@/components/AuthRequired';
import ActionRequired from '@/components/ActionRequired';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { cn } from '@/lib/utils';
import { Calendar } from '@/components/ui/calendar';
import { format } from "date-fns";
import updateAlbumReleaseDate from '@/api/musicCollections/updateAlbumReleaseDate';
import Image from 'next/image';
import Link from 'next/link';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import addNewAlbumCredit from '@/api/musicCollections/addNewAlbumCredit';

interface SongItem {
  key: string;
  title: string;
  audioFileName?: string;
  contentLength?: number;
  contentType?: string;
  songPublicId: string;
  isOpen: boolean;
}

const NewAlbumContent = () => {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { user, setUser } = useUser();
  const router = useRouter();
  const { data, isLoading } = useSWR(isReady && isAuthenticated && (user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) ? getOrCreateAlbumDraft.name : null, () => getOrCreateAlbumDraft(axiosPrivate, auth.userId!), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [songs, setSongs] = useState<SongItem[]>([]);
  const [albumTitle, setAlbumTitle] = useState(data?.data?.title ?? "");
  const [coverImageSrc, setCoverImageSrc] = useState("");
  const [albumTitleDb, setAlbumTitleDb] = useState(data?.data?.title ?? "");
  const [featuredArtists, setFeaturedArtists] = useState<GetOrCreateAlbumDraftCreditResponse[]>([]);
  const [submitted, setSubmitted] = useState(false);
  const [artistInput, setArtistInput] = useState('');
  const [searchResults, setSearchResults] = useState<GetArtistResponse[]>([])

  const [date, setDate] = useState(data?.data?.releaseDate ? new Date(data.data.releaseDate) : undefined)
  const [isUploading, setIsUploading] = useState(false)
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8, // Reduced from 10 to make it slightly more sensitive
      }
    }),
    useSensor(TouchSensor, {
      activationConstraint: {
        delay: 150, // Reduced from 250 to make it more responsive
        tolerance: 8, // Increased from 5 to allow for more finger movement
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDateSelect = async (selectedDate: Date | undefined) => {
    setIsUploading(true);
    const response = await updateAlbumReleaseDate(axiosPrivate, auth.userId!, data?.data?.albumId!, selectedDate ? format(selectedDate, "yyyy-MM-dd") : "");
    if (response.ok) {
      setDate(selectedDate);
    } else {
      setDate(data?.data?.releaseDate ? new Date(data.data.releaseDate) : undefined);
    }
    setIsUploading(false);
  }

  useEffect(() => {
    if (data?.ok) {
      setAlbumTitleDb(data.data?.title ?? "");
      setAlbumTitle(data.data?.title ?? "");
      setDate(data?.data?.releaseDate ? new Date(data.data.releaseDate) : undefined);
      setSongs(data.data?.songs.sort(x => x.sequence).map(x => ({
        key: x.key,
        title: x.title,
        songPublicId: x.songPublicId,
        audioFileName: x.originalFileName,
        contentLength: x.contentLength,
        contentType: x.contentType,
        isOpen: false
      })) ?? []);
      if (data.data?.coverImageS3Folder) {
        setCoverImageSrc(`${getURL()}api/music-collections/${data?.data?.albumPublicId!}/cover-image`);
      }
      const firstSong = data.data?.songs[0];
      if (firstSong) {
        setFeaturedArtists(data.data!.credits);
      }
    }
  }, [data]);

  const itemIds = useMemo(() => songs?.map((x) => x.key) ?? [], [songs]);

  const toggleSongAccordionItem = (key: any, forceClose?: boolean) => {
    const updatedSongs = songs.map((songItem) => {
      if (songItem.key === key) {
        return { ...songItem, isOpen: forceClose ? false : !songItem.isOpen };
      }
      return { ...songItem, isOpen: false };
    });
    setSongs(updatedSongs);
  };

  const updateSongTitle = async (key: string, newTitle: string) => {
    if (songs.find(x => x.key === key)?.title === newTitle) {
      return true;
    }

    const response = await updateAlbumSongTitle(axiosPrivate, auth.userId!, data?.data?.albumId!, key, newTitle);
    if (!response.ok) {
      return false;
    }

    const updatedSongs = songs.map((songItem) => {
      if (songItem.key === key) {
        return { ...songItem, title: newTitle };
      }
      return songItem;
    });

    setSongs(updatedSongs);

    return true;
  };

  const uploadAudio = async (key: string, file: File) => {
    const tokenResponse = await getAntiforgeryToken(axiosPrivate);
    if (!tokenResponse.ok) {
      return false;
    }

    const response = await uploadAudioFile(axiosPrivate, auth.userId!, key, file, tokenResponse.data!);
    if (!response.ok) {
      return false;
    }

    const updatedSongs = songs.map((songItem) => {
      if (songItem.key === key) {
        return {
          ...songItem,
          audioFileName: response.data?.originalFileName,
          contentLength: response.data?.contentLength,
          contentType: response.data?.contentType
        };
      }
      return songItem;
    });

    setSongs(updatedSongs);

    return true;
  };

  const onUploadCoverImage = async (file: File, fileName: string) => {
    const tokenResponse = await getAntiforgeryToken(axiosPrivate);
    if (!tokenResponse.ok) {
      return false;
    }

    const response = await uploadAlbumCoverImage(axiosPrivate, auth.userId!, data?.data?.albumId!, file, fileName, tokenResponse.data!);
    if (!response.ok) {
      return false;
    }

    setCoverImageSrc(`${getURL()}api/music-collections/${data?.data?.albumPublicId!}/cover-image?${Date.now()}`);

    return true;
  };

  const onDeleteCoverImage = async () => {
    const response = await deleteAlbumCoverImage(axiosPrivate, auth.userId!, data?.data?.albumId!);
    if (response.ok) {
      setCoverImageSrc("");
    }
  };

  const addArtist = async (artist: GetArtistResponse) => {
    const response = await addAlbumCredit(axiosPrivate, auth.userId!, data?.data?.albumId!, artist.artistId!);
    if (response.ok) {
      setFeaturedArtists(prevItems => [...prevItems, {
        id: response.data?.value!,
        profileName: artist.name,
        profileId: artist.artistId,
        profilePublicId: artist.publicId,
        profileUsername: artist.username
      }]);
    }
    setArtistInput('')
    setSearchResults([])
  }

  const addNewArtist = async () => {
    const response = await addNewAlbumCredit(axiosPrivate, auth.userId!, data?.data?.albumId!, artistInput);
    if (response.ok) {
      setFeaturedArtists(prevItems => [...prevItems, {
        id: response.data?.value!,
        profileName: artistInput
      }]);
    }
    setArtistInput('')
    setSearchResults([])
  }

  const removeArtist = async (id: string) => {
    const response = await deleteAlbumCredit(axiosPrivate, id, auth.userId!);
    if (response.ok) {
      setFeaturedArtists(prevItems => prevItems.filter(x => x.id !== id));
    }
  }

  const onFetchArtists = useCallback(async (query: string) => {
    if (!query) {
      return setSearchResults([]);
    }

    const data = await getArtists(axiosPrivate, query, auth.userId!);
    if (data.ok) {
      setSearchResults(data.data!.filter(artist => !featuredArtists.some(featuredArtist => featuredArtist.profileId === artist.artistId)));
    }
  }, [featuredArtists])

  useEffect(() => {
    if (!artistInput.length) {
      return;
    }
    const timer = setTimeout(() => {
      onFetchArtists(artistInput);
    }, 400)
    return () => clearTimeout(timer)
  }, [artistInput, onFetchArtists])

  const onAddNewSong = async () => {
    const response = await addAlbumSong(axiosPrivate, auth.userId!, data?.data?.albumId!);
    if (response.ok) {
      setSongs(prevItems => [...prevItems, {
        key: response.data!.songId,
        title: "",
        songPublicId: response.data!.songPublicId,
        isOpen: false
      }]);
    }
  };

  const onSongDelete = async (key: string) => {
    const response = await deleteAlbumSong(axiosPrivate, auth.userId!, data?.data?.albumId!, key);
    if (response.ok) {
      setSongs(prevItems => prevItems.filter(x => x.key !== key));
    }
  };

  const onAudioDelete = async (key: string) => {
    const response = await deleteAudioFile(axiosPrivate, auth.userId!, key);
    if (response.ok) {
      const updatedSongs = songs.map((songItem) => {
        if (songItem.key === key) {
          return { ...songItem, audioFileName: "", contentLength: undefined, contentType: undefined };
        }
        return songItem;
      });

      setSongs(updatedSongs);
    }
  };

  const onSubmit = async () => {
    const response = await submitAlbum(axiosPrivate, auth.userId!, data?.data?.albumId!);
    if (response.ok) {
      setSubmitted(true);
      toast({ title: 'Your album is submitted' });
      router.push('/dashboard');
    }
  };


  const updateAlbumTitle = async (value: string) => {
    if (albumTitleDb !== value) {
      const response = await updateMusicCollectionTitle(axiosPrivate, auth.userId!, data?.data?.albumId!, value);
      if (response.ok) {
        setAlbumTitleDb(value);
        toast({ title: 'Title saved' });
      } else {
        setAlbumTitle(albumTitleDb);
      }
    }
  };

  const handleSongDragEnd = async (event: any) => {
    const { active, over } = event;
    if (active.id !== over.id) {
      const activeItemIndex = songs.findIndex((x) => x.key === active.id);
      const overItemIndex = songs.findIndex((x) => x.key === over.id);
      setSongs((items) => {
        return arrayMove(items, activeItemIndex, overItemIndex);
      });
      const response = await updateMusicCollectionSongSequence(axiosPrivate, auth.userId!, data?.data?.albumId!, active.id, over.id);
      if (!response.ok) {
        setSongs((items) => {
          return arrayMove(items, overItemIndex, activeItemIndex);
        });
      }
    }
  }

  const handleSongDragStart = async (event: any) => {
    toggleSongAccordionItem(event.active.id, true);
  }

  if (!isReady || !user) {
    return <></>;
  }

  if (isLoading) {
    return (<div className="flex flex-col space-y-3 p-5">
      <Skeleton className="h-[325px] w-full  rounded-xl" />
      <div className="space-y-2">
        <Skeleton className="h-[55px] w-full " />
        <Skeleton className="h-[55px] w-full " />
      </div>
    </div>)
  }

  if (!isAuthenticated) {
    return <AuthRequired />
  }

  if ((user?.profileType !== ProfileType.Artist && user?.profileType !== ProfileType.Label)) {
    return <ActionRequired buttonText='Update profile' link='/settings' description='To upload your music please update your profile type to the Artist or Label' />
  }

  return (<Card>
    <CardHeader>
      <CardTitle>Album details</CardTitle>
      <CardDescription>
        Make changes to your album.
      </CardDescription>
    </CardHeader>
    <CardContent>
      <Label className='mt-2.5 mb-1 block'>Album name</Label>
      <Input
        value={albumTitle}
        onBlur={(e) => updateAlbumTitle(e.target.value)}
        onChange={(e) => setAlbumTitle(e.target.value.slice(0, 150))}
        placeholder='Type your album title'
        maxLength={150}
      />
      <Label className='mt-2.5 mb-1 block'>Release date</Label>
      <div className="flex flex-col items-start space-y-4">
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant={"outline"}
              className={cn(
                "w-full justify-start text-left font-normal ",
                !date && "text-muted-foreground"
              )}
            >
              <CalendarIcon className="mr-2 h-4 w-4" />
              {date ? format(date, "PPP") : <span>Pick a date</span>}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0">
            <Calendar
              mode="single"
              selected={date}
              onSelect={handleDateSelect}
              disabled={(date) => date > new Date() || date < new Date("1900-01-01")}
              initialFocus
            />
          </PopoverContent>
        </Popover>
      </div>
      <Label className='mt-2.5 mb-1 block'>Cover image</Label>
      <ImageUploader onUpload={onUploadCoverImage} uploadedImageUrl={coverImageSrc} onDelete={onDeleteCoverImage} />
      <Label className='mt-2.5 block'>{user.profileType === ProfileType.Artist ? "Collaborators" : "Main Artists"}</Label>
      <div className="space-y-1">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2 mt-2">
          {featuredArtists.map(artist => (
            <div
              key={artist.id}
              className="border px-1 py-1 rounded-full text-sm flex items-center justify-between w-full"
            >
              {artist.profileId && (
                <Avatar className="h-7 w-7 mr-2">
                  <AvatarImage src={getProfileImageSrc(artist?.profilePublicId)} alt={artist.profileName} />
                  <AvatarFallback>{artist.profileName.slice(0, 2).toUpperCase()}</AvatarFallback>
                </Avatar>
              )}
              {artist.profilePublicId && (
                <Link
                  className="hover:underline"
                  href={`/profiles/${artist.profileUsername!.toLowerCase()}`}
                  key={`/profiles/${artist.profileUsername!.toLowerCase()}`}
                  title={`View ${artist.profileName}'s profile`}
                >
                  {artist.profileName}
                </Link>
              )}
              {!artist.profilePublicId && (
                <UserPlus className="h-4 w-4" />
              )}
              {!artist.profilePublicId && (
                <span className='truncate'>{artist.profileName}</span>
              )}
              <Button
                variant="ghost"
                size="icon"
                className='rounded-full'
                onClick={() => removeArtist(artist.id)}
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          ))}
        </div>
        <div className="relative">
          <Input
            placeholder="Search or add new artist"
            value={artistInput}
            onChange={(e) => setArtistInput(e.target.value.slice(0, 50))}
            maxLength={50}
          />
          {(searchResults.length > 0 || artistInput.trim()) && (
            <div className="absolute z-10 w-full mt-1 bg-popover border rounded-md shadow-md">
              {artistInput.trim() && !featuredArtists.some(artist =>
                artist.profileName.toLowerCase() === artistInput.trim().toLowerCase()
              ) && (
                  <div
                    className="p-2 hover:bg-accent cursor-pointer flex items-center justify-between"
                    onClick={addNewArtist}
                  >
                    <span>Add "{artistInput}" as new artist</span>
                    <Plus className="h-4 w-4" />
                  </div>
                )}
              {searchResults.map(artist => (
                <div
                  key={artist.artistId}
                  className="p-2 hover:bg-accent cursor-pointer flex items-center"
                  onClick={() => addArtist(artist)}
                >
                  <Avatar className="h-7 w-7 mr-2">
                    <AvatarImage src={getProfileImageSrc(artist.publicId)} alt={artist.name} />
                    <AvatarFallback>{artist.name.slice(0, 2).toUpperCase()}</AvatarFallback>
                  </Avatar>
                  {artist.name}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
      <div>
        <Label className='mt-2.5 mb-1 block'>Songs</Label>
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleSongDragEnd}
        //onDragStart={handleSongDragStart}
        >
          <SortableContext
            items={itemIds}
            strategy={verticalListSortingStrategy}
          >
            {songs.map((songItem, index) => (
              <SongAccordionItem
                id={songItem.key}
                key={songItem.key}
                isOpen={songItem.isOpen}
                toggleAccordion={() => toggleSongAccordionItem(songItem.key)}
                title={songItem.title}
                onTitleUpdate={(newTitle) => updateSongTitle(songItem.key, newTitle)}
                onAudioUpload={(file) => uploadAudio(songItem.key, file)}
                songPublicId={songItem.songPublicId}
                audioFileName={songItem.audioFileName}
                contentLength={songItem.contentLength}
                contentType={songItem.contentType}
                onSongDelete={() => onSongDelete(songItem.key)}
                onAudioDelete={() => onAudioDelete(songItem.key)}
              />
            ))}
          </SortableContext>
        </DndContext>
        <Button onClick={onAddNewSong} variant='ghost' className="w-full">
          <Plus size={18} />
          <span>Add song</span>
        </Button>
      </div>
      <AlertDialog>
        <AlertDialogTrigger asChild>
          {/* {submitted && <Button disabled={submitted} variant="outline">Edit</Button>} */}
          <PurpleButton className='px-6 py-2 mt-2' disabled={submitted}>{submitted ? "Submitted & Processing" : "Submit"}</PurpleButton>
        </AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Ready to release your album?</AlertDialogTitle>
            <AlertDialogDescription>
              Once submitted, this action cannot be undone. Your album will be processed and published.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={onSubmit}>Continue</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </CardContent>
  </Card>);
};

export default NewAlbumContent;
