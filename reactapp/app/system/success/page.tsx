import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../../loading';
import dynamic from 'next/dynamic';
import SuccessContent from './components/SuccessContent';

const Success = async () => {
  // const { isPageLoading } = usePageLoading();
  // return isPageLoading ? <Loading /> : <NewAlbumContent />;
  return <SuccessContent />;
};

export default Success;
