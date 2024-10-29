import MusicCollectionContent from './components/MusicCollectionContent';

const MusicCollection = async ({ params }: { params: { collectionPublicId: string } }) => {
  return (
    <div className='my-0 mx-auto max-w-full h-full'>
      <MusicCollectionContent collectionPublicId={params.collectionPublicId} />
    </div>
  );
};

export default MusicCollection;
