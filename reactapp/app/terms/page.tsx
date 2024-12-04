import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../loading';
import dynamic from 'next/dynamic';
import TermsContent from './components/TermsContent';
import { Metadata } from 'next';

export const metadata: Metadata = {
  title: 'Terms, Conditions & Privacy',
  description: 'Read the terms and conditions for using our music streaming platform. Learn about user rights, royalties, subscriptions, and content policies.',
  openGraph: {
    title: 'Terms, Conditions & Privacy | Noizera',
    description: 'Read the terms and conditions for using our music streaming platform. Learn about user rights, royalties, subscriptions, and content policies.',
  },
};

const Terms = async () => {
  return (
    <div className='my-0 mx-auto max-w-3xl min-h-full'>
      <TermsContent />
    </div>
  );
};

export default Terms;
