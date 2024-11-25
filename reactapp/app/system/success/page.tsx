import SuccessContent from './components/SuccessContent';

const Success = async ({
  searchParams,
}: {
  searchParams: Promise<{ session_id: string }>
}) => {
  const { session_id } = await searchParams;
  return <SuccessContent sessionId={session_id} />;
};

export default Success;
