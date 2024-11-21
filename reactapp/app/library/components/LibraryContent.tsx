'use client';

import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import useAuth from '@/hooks/useAuth';
import useLibrary from '@/hooks/useLibrary';
import { MusicArtwork } from '@/components/MusicArtwork';
import AuthRequired from '@/components/AuthRequired';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import PurpleButton from '@/components/Button';
import { useRouter } from 'next/navigation';

const LibraryContent: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const router = useRouter();
  const { collections } = useLibrary();
  const { isReady } = useAxiosPrivate();

  if (!isReady) {
    return <></>;
  }

  if (!isAuthenticated) {
    return <AuthRequired />
  }

  if (collections.length === 0) {
    return <Card className="w-full">
      <CardHeader className="text-center">
        <CardTitle className="text-2xl md:text-3xl">Add new music to your library</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-center text-muted-foreground mb-6">
          Let's listen to new music.
        </p>
      </CardContent>
      <CardFooter className="flex flex-col space-y-4">
        <PurpleButton
          onClick={() => router.push("/")}
          className='px-6 py-2'
        >
          Listen
        </PurpleButton>
      </CardFooter>
    </Card>
  }

  return (
    <div>
      <div className="mt-6 space-y-1">
        <h2 className="text-2xl font-semibold tracking-tight">
          Your Music Library
        </h2>
      </div>
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
        {collections.map((collection) => (
          <div className='mt-4' key={collection.publicId}>
            <MusicArtwork
              title={collection.title}
              publicId={collection.publicId}
              collectionType={collection.collectionType}
              isSaved={true}
              ownerName={collection.ownerName}
              ownerUsername={collection.ownerPublicId}
              songCount={collection.songCount}
            />
          </div>
        ))}
      </div>
    </div>

  );
};

export default LibraryContent;
