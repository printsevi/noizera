import MusicCollectionContent from './components/MusicCollectionContent';

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
