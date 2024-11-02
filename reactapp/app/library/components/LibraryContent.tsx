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

  return (
    <div>
      <div className="mt-6 space-y-1">
        <h2 className="text-2xl font-semibold tracking-tight">
          Your Music Library
        </h2>
      </div>
      {collections.length === 0 && <Card className="w-full">
        <CardHeader className="text-center">
          <CardTitle className="text-2xl md:text-3xl">Add new music</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-center text-muted-foreground mb-6">
            Listen to new music.
          </p>
        </CardContent>
        <CardFooter className="flex flex-col space-y-4">
          <PurpleButton
            onClick={() => router.push("/")}
            className='bg-white px-6 py-2'
          >
            New music
          </PurpleButton>
        </CardFooter>
      </Card>}
      {collections.length > 0 && <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
        {collections.map((collection) => (
          <div className='mt-4' key={collection.publicId}>
            <MusicArtwork
              title={collection.title}
              publicId={collection.publicId}
              collectionType={collection.collectionType}
              isSaved={true} />
          </div>
        ))}
      </div>}
    </div>

  );
};

export default LibraryContent;
