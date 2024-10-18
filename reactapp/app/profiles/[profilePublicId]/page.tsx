import ProfileContent from './components/ProfileContent';

const Profile = async ({ params }: { params: { profilePublicId: string } }) => {
  return (
    <div className='my-0 mx-auto max-w-full h-full'>
                  <ProfileContent profilePublicId={params.profilePublicId} />
                </div>
  );
};

export default Profile;
