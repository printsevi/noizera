import { Metadata } from 'next';
import NewAlbumContent from './components/NewAlbumContent';

export const metadata: Metadata = {
  title: 'Release your music',
  description: 'Release your music on our independent music streaming platform',
  robots: {
    index: false,  // Do not index this page
    follow: false, // Do not follow links on this page
  }
};

const NewAlbum = async () => {
  return (
    <div className='mx-auto max-w-2xl min-h-full p-3'>
      <NewAlbumContent />
    </div>
  );
};

export default NewAlbum;
