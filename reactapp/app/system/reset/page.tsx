import Header from '@/components/Header';
import { usePageLoading } from '@/hooks/usePageLoading';
import Loading from '../../loading';
import dynamic from 'next/dynamic';
import ResetContent from './components/ResetContent';

const Success = async () => {
  // const { isPageLoading } = usePageLoading();
  // return isPageLoading ? <Loading /> : <NewAlbumContent />;
  return <ResetContent />;
};

export default Success;
