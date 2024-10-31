import SearchContent from './components/SearchContent';

export const revalidate = 0;

interface Props {
  searchParams: { query: string };
}

const Search = async ({ searchParams }: Props) => {
  return (
    <div className='min-h-full px-3'>
      <SearchContent query={searchParams.query} />
    </div>
  );
};

export default Search;
