import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../../loading';
import dynamic from 'next/dynamic';
import ResetContent from './components/ResetContent';
import { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'Reset password',
  description: 'Reset your password',
  robots: {
    index: false,
    follow: false,
  }
};

const ResetPage = async ({
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

export default ResetPage;
