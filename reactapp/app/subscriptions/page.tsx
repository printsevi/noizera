import { Metadata } from 'next';
import SubscriptionsContent from './components/SubscriptionsContent';

export const metadata: Metadata = {
  title: 'Subscriptions',
  description: 'Choose an affordable music subscription. Stream FLAC-quality tracks, enjoy curated playlists, and support independent artists with high royalties.',
  openGraph: {
    title: 'Subscriptions | Noizera',
    description: 'Choose an affordable music subscription. Stream FLAC-quality tracks, enjoy curated playlists, and support independent artists with high royalties.',
  },
};

const Subscriptions = async () => {
  return (<div className='min-h-full my-0 mx-auto max-w-xl px-3'>
    <SubscriptionsContent />
  </div>)
};

export default Subscriptions;
