import { Metadata } from "next";
import PageContent from "./components/PageContent";

export const revalidate = 0;

export const metadata: Metadata = {
  title: 'Independent Music Streaming',
  description: 'Discover a music streaming platform for independent artists and labels offering high royalties. Enjoy FLAC-quality songs, curated playlists, and affordable subscriptions.',
  openGraph: {
    title: 'Noizera | Independent Music Streaming',
    description: 'Discover a music streaming platform for independent artists and labels offering high royalties. Enjoy FLAC-quality songs, curated playlists, and affordable subscriptions.',
  },
};

export default async function Home() {
  return (
    <div className='min-h-full px-6'>
      <PageContent />
    </div>
  );
}
