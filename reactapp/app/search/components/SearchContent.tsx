'use client';

import { Song } from '@/types';
import useOnPlay from '@/hooks/useOnPlay';
import { useSearchParams } from 'next/navigation';
import useSWR from 'swr';
import { useEffect, useState } from 'react';
import searchPublic, { SearchQueryResult } from '@/api/search/searchPublic';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import Image from 'next/image';
import { getCoverImageSrc, getProfileImageSrc } from '@/libs/helpers';
import { cn } from '@/lib/utils';
import ImageWithFallback from '@/components/ImageWithFallback';

interface Props {
  query: string;
}

const SearchContent: React.FC<Props> = ({ query }) => {
  const { data, isLoading } = useSWR(searchPublic.name + query, () => searchPublic(query), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [searchResults, setSearchResults] = useState<SearchQueryResult[]>([]);
  const [showAllProfiles, setShowAllProfiles] = useState(false)
  const [showAllAlbums, setShowAllAlbums] = useState(false)

  useEffect(() => {
    if (data?.ok) {
      setSearchResults(data.data!);
    }
  }, [data]);

  const groupedResults = searchResults.reduce((acc, result) => {
    if (!acc[result.category]) {
      acc[result.category] = []
    }
    acc[result.category].push(result)
    return acc
  }, {} as Record<string, SearchQueryResult[]>)

  const renderResults = (category: string, items: SearchQueryResult[]) => {
    const isProfile = category === 'Profiles'
    const showAll = isProfile ? showAllProfiles : showAllAlbums
    const visibleItems = showAll ? items : items.slice(0, 3)

    return (
      <div key={category}>
        <div className="mt-6 space-y-1">
          <h2 className="text-2xl font-semibold tracking-tight">
            {isProfile ? "Artists & Labels" : "Albums & Singles"}
          </h2>
        </div>
        <ul className="space-y-6">
          {visibleItems.map((item, index) => (
            <li key={item.publicId}>
              <div className="flex items-center space-x-4 mb-4">
                <Link
                  href={`/${isProfile ? 'profiles' : 'collections'}/${item.publicId.toLowerCase()}`}
                  className={`relative ${isProfile ? 'w-20 h-20 rounded-full' : 'w-20 h-20 rounded-lg'} overflow-hidden group`}
                >
                  <ImageWithFallback
                    src={isProfile ? getProfileImageSrc(item.imageId) : getCoverImageSrc(item.imageId)}
                    alt={item.title}
                    fallbackSrc="/images/user.png"
                    width={80}
                    height={80}
                    className={`object-cover ${isProfile ? 'rounded-full' : 'rounded-lg'} transition-opacity group-hover:opacity-80`}
                  />
                </Link>
                <Link
                  href={`/${isProfile ? 'profiles' : 'collections'}/${item.publicId.toLowerCase()}`}
                  className="flex-1 min-w-0"
                >
                  <span className={cn(
                    "block font-semibold hover:underline",
                    item.title.length > 20 ? "text-lg line-clamp-2 text-ellipsis" : "text-xl truncate"
                  )}>
                    {item.title}
                  </span>
                  {/* <span className="block text-sm text-gray-500 truncate">
                    {isProfile ? 'Artist' : 'Album'}
                  </span> */}
                </Link>
              </div>
              {index < visibleItems.length - 1 && <Separator className="my-4" />}
            </li>
          ))}
        </ul>
        {items.length > 3 && (
          <Button
            variant="outline"
            className="mt-6 rounded-full"
            onClick={() => isProfile ? setShowAllProfiles(!showAll) : setShowAllAlbums(!showAll)}
          >
            {showAll ? 'Show Less' : 'Show All'}
          </Button>
        )}
      </div>
    )
  }

  if (searchResults.length === 0 && !isLoading) {
    return (<div>Nothing found</div>);
  }

  return (
    <div className="space-y-12">
      {Object.entries(groupedResults).map(([category, items]) => renderResults(category, items))}
    </div>
  )
};

export default SearchContent;
