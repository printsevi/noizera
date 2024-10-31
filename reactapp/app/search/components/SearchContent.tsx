'use client';

import { Song } from '@/types';
import useOnPlay from '@/hooks/useOnPlay';
import { useSearchParams } from 'next/navigation';

interface Props {
  query: string;
}

const SearchContent: React.FC<Props> = ({ query }) => {
  return (
    <div
      className='
        flex 
        flex-col 
        gap-y-2 
        w-full 
        px-6 
        text-neutral-400
      '
    >
      {query}
    </div>
  );

  return (
    <div className='flex flex-col gap-y-2 w-full px-6'>
    </div>
  );
};

export default SearchContent;
