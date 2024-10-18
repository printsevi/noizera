import LibraryContent from './components/LibraryContent';

export const revalidate = 0;

const LibraryPage = async () => {
  return (
    <div className='w-fit my-0 mx-auto max-w-full h-full'>
      <LibraryContent/>
    </div>
  );
};

export default LibraryPage;
