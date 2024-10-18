'use client';

import { Song } from '@/types';
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from '@/components/ui/carousel';
import { Card, CardContent } from '@/components/ui/card';
import { MusicArtwork } from '@/components/MusicArtwork';
import { getURL } from '@/libs/helpers';
import { useCallback, useEffect, useState } from 'react';
import useAuth from '@/hooks/useAuth';
import getFeedCategories, { GetFeedMusicCollectionItem } from '@/api/feed/getFeedCategories';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import getFeedPublicCategories from '@/api/feed/getFeedPublicCategories';
import getFeedPublicCollections from '@/api/feed/getFeedPublicCollections';
import { AxiosInstance } from 'axios';
import getFeedCollections from '@/api/feed/getFeedCollections';
import Image from 'next/image';

interface PageContentProps {
  songs: Song[];
}

interface FeedMusicCategoryItem {
  title: string;
  api: string;
  items: MusicCollectionResult[];
}

interface MusicCollectionResult {
  publicId: string,
  title: string,
  collectionType: string
}

const fetchCollectionsForPublicCategory = async (category: GetFeedMusicCollectionItem)
  : Promise<FeedMusicCategoryItem> => {
  const publicCollectionsData = await getFeedPublicCollections(category.api);
  if (!publicCollectionsData.ok) {
    return { ...category, items: [] };
  }

  return { ...category, items: publicCollectionsData.data?.items! };
};

const fetchCollectionsForCategory = async (category: GetFeedMusicCollectionItem, userId: string, axiosPrivate: AxiosInstance)
  : Promise<FeedMusicCategoryItem> => {
  const publicCollectionsData = await getFeedCollections(category.api, userId, axiosPrivate);
  if (!publicCollectionsData.ok) {
    return { ...category, items: [] };
  }

  return { ...category, items: publicCollectionsData.data?.items! };
};

const PageContent: React.FC<PageContentProps> = ({ songs }) => {
  const { auth, isAuthenticated } = useAuth();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [musicCategories, setMusicCategories] = useState<FeedMusicCategoryItem[]>([]);

  const fetchFeed = useCallback(async () => {
    if (!isReady) return;
    let apiMusicCategories: FeedMusicCategoryItem[] = [];
    if (isAuthenticated) {
      const data = await getFeedCategories(axiosPrivate, auth.userId!);
      if (data.ok) {
        apiMusicCategories = await Promise.all(
          data.data!.musicCategories.map(category => fetchCollectionsForCategory(category, auth.userId!, axiosPrivate))
        );
      }
    } else {
      const data = await getFeedPublicCategories();
      if (data.ok) {
        apiMusicCategories = await Promise.all(
          data.data!.musicCategories.map(category => fetchCollectionsForPublicCategory(category))
        );
      }
    }
    setMusicCategories(apiMusicCategories);
  }, [isAuthenticated, isReady, axiosPrivate, auth.userId]);

  useEffect(() => {
    if (isReady) {
      fetchFeed();
    }
  }, [isReady, isAuthenticated, fetchFeed]);


  if (musicCategories.length === 0) {
    return <div></div>
  }

  return (
    <div>
      {musicCategories.map((category) => (
        <section key={category.api} className="space-y-4">
          <div className="mt-6 space-y-1">
            <h2 className="text-2xl font-semibold tracking-tight">
              {category.title}
            </h2>
            <p className="text-sm text-muted-foreground">
              Your personal playlists. Updated daily.
            </p>
          </div>
          <Carousel
            opts={{
              align: "start",
              loop: true,
            }}
            className="relative w-full"
          >
            <CarouselPrevious className='absolute left-0 top-1/2 -translate-y-1/2' />
            <CarouselNext className="absolute right-0 top-1/2 -translate-y-1/2" />
            <CarouselContent>
              {category.items?.map((item, index) => (
                <CarouselItem key={item.publicId} className="pl-2 md:pl-4 basis-1/2 md:basis-1/5">
                  <MusicArtwork
                    title={item.title}
                    publicId={item.publicId}
                    coverPath={`${getURL()}api/music-collections/${item.publicId}/cover-image`} />

                </CarouselItem>
              ))}
              {Array.from({ length: 20 - (category.items?.length ?? 0) }).map((_, index) => (
                <CarouselItem key={index} className="basis-1/2 pl-2 sm:basis-1/2 md:basis-1/3 lg:basis-1/5 md:pl-4">
                  <Image
                    src={`${getURL()}api/music-collections/pdv/cover-image`}
                    alt="placeholder-image"
                    width={500}
                    height={500}
                    className="w-full h-auto aspect-square object-cover rounded-md"
                  />
                </CarouselItem>
              ))}
            </CarouselContent>
          </Carousel>
        </section>
      ))}
    </div>
  );
}

export default PageContent;
