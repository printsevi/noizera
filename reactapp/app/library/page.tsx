import LibraryContent from './components/LibraryContent';

export const revalidate = 0;

const LibraryPage = async () => {
  return (
    <div className='min-h-full px-6'>
      <LibraryContent />
    </div>
  );
};

export default LibraryPage;
