import { Metadata } from 'next';
import SearchContent from './components/SearchContent';

export const revalidate = 0;

export const metadata: Metadata = {
  title: 'Search',
  description: 'Search for songs, albums, and independent artists. Find trending tracks, curated playlists, and high-quality FLAC music tailored to your preferences.',
  robots: {
    index: false,  // Do not index this page
    follow: false, // Do not follow links on this page
  }
};

const Search = async ({
  searchParams,
}: {
  searchParams: Promise<{ query: string }>
}) => {
  const { query } = await searchParams;
  return (
    <div className='min-h-full px-6'>
      <SearchContent query={query} />
    </div>
  );
};

export default Search;
