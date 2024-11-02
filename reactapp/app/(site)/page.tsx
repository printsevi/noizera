import PageContent from "./components/PageContent";

export const revalidate = 0;

export default async function Home() {
  return (
    <div className='min-h-full px-6'>
      <PageContent />
    </div>
  );
}
