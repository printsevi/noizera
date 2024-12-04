import { Metadata } from 'next';
import MusicCollectionContent from './components/MusicCollectionContent';

export const metadata: Metadata = {
  title: 'Music Collection',
  description: 'Explore [Album Name] by [Artist Name]. Stream FLAC-quality tracks, discover lyrics, and enjoy music from your favorite independent artists and labels.',
  openGraph: {
    title: 'Music Collection | Noizera',
    description: 'Explore [Album Name] by [Artist Name]. Stream FLAC-quality tracks, discover lyrics, and enjoy music from your favorite independent artists and labels.',
  },
};

const MusicCollection = async ({
  params,
}: {
  params: Promise<{ collectionPublicId: string }>
}) => {
  const { collectionPublicId } = await params;
  return (
    <div className='container mx-auto px-4 py-8 min-h-full'>
      <MusicCollectionContent collectionPublicId={collectionPublicId} />
    </div>
  );
};

export default MusicCollection;
