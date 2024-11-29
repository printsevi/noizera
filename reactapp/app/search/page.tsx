import SearchContent from './components/SearchContent';

export const revalidate = 0;

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
