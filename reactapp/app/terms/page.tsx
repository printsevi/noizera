import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../loading';
import dynamic from 'next/dynamic';
import TermsContent from './components/TermsContent';

const Terms = async () => {
  return (
    <div className='my-0 mx-auto max-w-3xl'>
      <TermsContent />
    </div>
  );
};

export default Terms;
