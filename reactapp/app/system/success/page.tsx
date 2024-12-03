import SuccessContent from './components/SuccessContent';

const Success = async ({
  searchParams,
}: {
  searchParams: Promise<{ session_id: string }>
}) => {
  const { session_id } = await searchParams;
  return (
    <div className='my-0 mx-auto max-w-xl min-h-full'>
      <SuccessContent sessionId={session_id} />
    </div>
  );
};

export default Success;
