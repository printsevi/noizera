import MusicCollectionContent from './components/MusicCollectionContent';

const MusicCollection = async ({
  params,
}: {
  params: Promise<{ collectionPublicId: string }>
}) => {
  const { collectionPublicId } = await params;
  return (
    <div className='my-0 mx-auto max-w-5xl h-full'>
      <MusicCollectionContent collectionPublicId={collectionPublicId} />
    </div>
  );
};

export default MusicCollection;
