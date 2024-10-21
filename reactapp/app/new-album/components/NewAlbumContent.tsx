'use client';

import React, { useEffect, useMemo, useState } from 'react';
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
import getOrCreateAlbumDraft from '@/api/musicCollections/getOrCreateAlbumDraft';
import useSWR from 'swr';
import Box from '@/components/Box';
import updateMusicCollectionTitle from '@/api/musicCollections/updateMusicCollectionTitle';
import getArtists from '@/api/profiles/getArtists';
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
import { getURL } from '@/libs/helpers';
import PurpleButton from '@/components/Button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from '@/hooks/use-toast';
import { useRouter } from 'next/navigation';
import { Skeleton } from '@/components/ui/skeleton';
import useUser from '@/hooks/useUser';
import { Plus } from 'lucide-react';
import deleteAudioFile from '@/api/songs/deleteAudioFile';
import deleteAlbumCoverImage from '@/api/musicCollections/deleteAlbumCoverImage';
import AuthRequired from '@/components/AuthRequired';
import ActionRequired from '@/components/ActionRequired';

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
  const [submitted, setSubmitted] = useState(false);

  useEffect(() => {
    if (data?.ok) {
      setAlbumTitleDb(data.data?.title ?? "");
      setAlbumTitle(data.data?.title ?? "");
      setSongs(data.data?.songs.sort(x => x.sequence).map(x => ({
        key: x.key,
        title: x.title,
        songPublicId: x.songPublicId,
        audioFileName: x.originalFileName,
        contentLength: x.contentLength,
        contentType: x.contentType,
        isOpen: false
      })) ?? []);
      if (data.data?.coverImageMongoId) {
        setCoverImageSrc(`${getURL()}api/music-collections/${data?.data?.albumPublicId!}/cover-image`);
      }

    }
  }, [data]);

  const itemIds = useMemo(() => songs?.map((x) => x.key) ?? [], [songs]);

  const toggleSongAccordionItem = (key: any) => {
    const updatedSongs = songs.map((songItem) => {
      if (songItem.key === key) {
        return { ...songItem, isOpen: !songItem.isOpen };
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

  const fetchArtists = async (text: string) => {
    const result: ComboboxItemProps[] = [];
    if (!text) {
      return result;
    }

    const data = await getArtists(axiosPrivate, text, auth.userId!,);
    if (data.ok) {
      data.data?.artists.forEach((x) => {
        result.push({ key: x.artistId, value: x.name });
      })
    }

    return result;
  };

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
      router.push('/');
    }
  };


  const updateAlbumTitle = async (value: string) => {
    console.log(value);
    if (albumTitleDb !== value) {
      const response = await updateMusicCollectionTitle(axiosPrivate, auth.userId!, data?.data?.albumId!, value);
      if (response.ok) {
        setAlbumTitleDb(value);
      } else {
        setAlbumTitle(albumTitleDb);
      }
    }
  };

  const onAddAlbumCredit = async (item: ComboboxItemProps) => {
    const response = await addAlbumCredit(axiosPrivate, auth.userId!, data?.data?.albumId!, item.key!);
    return response.data?.value;
  };

  const onDeleteAlbumCredit = async (creditId?: string) => {
    if (!creditId) {
      return false;
    }
    const response = await deleteAlbumCredit(axiosPrivate, creditId, auth.userId!);
    return response.ok;
  };

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 10,
      }
    }),
    useSensor(TouchSensor, {
      activationConstraint: {
        delay: 250,
        tolerance: 5,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

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

  if (!isReady) {
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

  return (<div className="min-h-screen bg-background p-4 sm:p-8">
    <div className="max-w-3xl mx-auto">
      <Tabs defaultValue="album-details" className='w-full'>
        <TabsList className="grid w-full grid-cols-2">
          <TabsTrigger value="album-details">Album details</TabsTrigger>
          <TabsTrigger value="song-details">Songs</TabsTrigger>
        </TabsList>
        <TabsContent value="album-details">
          <Card>
            <CardHeader>
              <CardTitle>Album details</CardTitle>
              <CardDescription>
                Make changes to your album details.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-2">
              <Label>Album name</Label>
              <Input
                value={albumTitle}
                onBlur={(e) => updateAlbumTitle(e.target.value)}
                onChange={(e) => setAlbumTitle(e.target.value)}
              />
              <Label>Cover image</Label>
              <ImageUploader onUpload={onUploadCoverImage} uploadedImageUrl={coverImageSrc} onDelete={onDeleteCoverImage} />
              {/* <ComboboxField 
              id="album-artists-combobox" 
              label={data?.data?.profileType === ProfileType.Artist ? 'Featured artists' : 'Artists'}
              initialSelectedItems={data?.data?.credits ?? []} 
              apiUrl='profiles/artists'
              getItems={fetchArtists}
              onItemAdd={onAddAlbumCredit}
              onItemDelete={onDeleteAlbumCredit}
            /> */}
            </CardContent>
          </Card>
        </TabsContent>
        <TabsContent value="song-details">
          <Card>
            <CardHeader>
              <CardTitle>Songs</CardTitle>
              <CardDescription>
                Add, sort and delete you songs here.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-2">
              <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragEnd={handleSongDragEnd}
              >
                <SortableContext
                  items={itemIds}
                  strategy={verticalListSortingStrategy}
                >
                  <div>
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
                  </div>
                </SortableContext>
              </DndContext>
              <Button onClick={onAddNewSong} variant='ghost' className="w-full">
                <Plus size={18} />
                <span>Add song</span>
              </Button>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
    <AlertDialog>
      <AlertDialogTrigger asChild>
        {/* {submitted && <Button disabled={submitted} variant="outline">Edit</Button>} */}
        <PurpleButton className='bg-white px-6 py-2' disabled={submitted}>Submit album</PurpleButton>
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Is the album ready to be submitted?</AlertDialogTitle>
          <AlertDialogDescription>
            This action cannot be undone. The album will be released after required checks.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction onClick={onSubmit}>Continue</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  </div>);
};

export default NewAlbumContent;
