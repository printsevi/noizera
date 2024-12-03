import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../../loading';
import dynamic from 'next/dynamic';
import ResetContent from './components/ResetContent';

const Success = async ({
  searchParams,
}: {
  searchParams: Promise<{ email: string, token: string }>
}) => {
  const { email, token } = await searchParams;
  return (
    <div className='my-0 mx-auto max-w-xl min-h-full'>
      <ResetContent email={email} token={token} />
    </div>
  );
};

export default Success;
