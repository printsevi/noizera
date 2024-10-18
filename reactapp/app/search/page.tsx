import getSongsByTitle from '@/api/getSongsByTitle';
import SearchInput from '@/components/SearchInput';
import Header from '@/components/Header';
import SearchContent from './components/SearchContent';

export const revalidate = 0;

interface ExploreProps {
  searchParams: { title: string };
}

const Explore = async ({ searchParams: exploreParams }: ExploreProps) => {
  const songs = await getSongsByTitle(exploreParams.title);

  return (
    <div
      className='
        bg-neutral-900 
        rounded-lg 
        h-full 
        w-full 
        overflow-hidden 
        overflow-y-auto
      '
    >
      <SearchContent songs={songs} />
    </div>
  );
};

export default Explore;
