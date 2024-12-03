import ProfileContent from './components/ProfileContent';

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
