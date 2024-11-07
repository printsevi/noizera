import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../../loading';
import dynamic from 'next/dynamic';
import ResetContent from './components/ResetContent';

const Success = async () => {
  return (
    <div className='my-0 mx-auto max-w-xl'>
      <ResetContent />
    </div>
  );
};

export default Success;
