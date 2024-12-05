import { Metadata } from 'next';
import OnboardingRefreshContent from './components/OnboardingRefreshContent';

export const metadata: Metadata = {
  title: 'Onboarding refreshed',
  description: 'your onboarding is refreshed',
  robots: {
    index: false,
    follow: false,
  }
};

const Success = async ({
  searchParams,
}: {
  searchParams: Promise<{ session_id: string }>
}) => {
  const { session_id } = await searchParams;
  return (
    <div className='my-0 mx-auto max-w-xl min-h-full'>
      <OnboardingRefreshContent />
    </div>
  );
};

export default Success;
