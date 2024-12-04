import { Metadata } from 'next';
import ProfileContent from './components/ProfileContent';

export const metadata: Metadata = {
  title: 'Profile',
  description: `View [Artist Name]'s profile. Discover their top tracks, albums, and curated playlists. Support independent artists through our high-royalty platform.`,
  openGraph: {
    title: 'Profile | Noizera',
    description: `View [Artist Name]'s profile. Discover their top tracks, albums, and curated playlists. Support independent artists through our high-royalty platform.`,
  },
};

const Profile = async ({
  params,
}: {
  params: Promise<{ profilePublicId: string }>
}) => {
  const { profilePublicId } = await params;
  return (
    <div className='my-0 mx-auto max-w-full min-h-full'>
      <ProfileContent username={profilePublicId} />
    </div>
  );
};

export default Profile;
