'use client';

import { Song } from '@/types';
import useOnPlay from '@/hooks/useOnPlay';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import useAuth from '@/hooks/useAuth';
import { useRouter } from 'next/router';
import useSWR from 'swr';
import useLibrary from '@/hooks/useLibrary';
import { IMusicCollectionModel } from '@/providers/LibraryProvider';

const LibraryContent: React.FC = () => {
  const { auth } = useAuth();
  const { collections } = useLibrary();

  if (!auth.accessToken) {
    return (
      <div
        className='
          flex 
          flex-col 
          gap-y-2 
          w-full 
          px-6 
          text-neutral-400
        '
      >
        Please sign up
      </div>
    );
  }

  if (auth.accessToken && collections.length === 0) {
    return (
      <div
        className='
          flex 
          flex-col 
          gap-y-2 
          w-full 
          px-6 
          text-neutral-400
        '
      >
        No songs found.
      </div>
    );
  }

  return (
    <div className='flex flex-col gap-y-2 w-full px-6'>
      {collections.map((collection) => (
        <div key={collection.publicId} className='flex items-center gap-x-4 w-full'>
          <div className='flex-1'>
            {/* <MediaItem onClick={(id: string) => onPlay(id)} data={song} /> */}
          </div>
          {/* <LikeButton songId={song.id} /> */}
        </div>
      ))}
    </div>
  );
};

export default LibraryContent;
