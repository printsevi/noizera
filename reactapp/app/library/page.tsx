import { Metadata } from 'next';
import LibraryContent from './components/LibraryContent';

export const revalidate = 0;

export const metadata: Metadata = {
  title: 'Your Library',
  description: 'Access your personal music library. Stream your saved albums, playlists, and favorite tracks in FLAC quality on any device, anytime.',
  robots: {
    index: false,  // Do not index this page
    follow: false, // Do not follow links on this page
  }
};

const LibraryPage = async () => {
  return (
    <div className='my-0 mx-auto min-h-full px-6'>
      <LibraryContent />
    </div>
  );
};

export default LibraryPage;
