import MusicCollectionContent from './components/MusicCollectionContent';

const MusicCollection = async ({ params }: { params: { profilePublicId: string } }) => {
  return (
    <div className='my-0 mx-auto max-w-full h-full'>
                  <MusicCollectionContent profilePublicId={params.profilePublicId} />
                </div>
  );
};

export default MusicCollection;
