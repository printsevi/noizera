import { Metadata } from "next";
import AboutContent from "./components/AboutContent";

export const metadata: Metadata = {
  title: 'About',
  description: 'Discover our mission to empower independent artists and labels. Learn how we provide high royalties, FLAC-quality streaming, and curated playlists for music lovers.',
  openGraph: {
    title: 'About | Noizera',
    description: 'Discover our mission to empower independent artists and labels. Learn how we provide high royalties, FLAC-quality streaming, and curated playlists for music lovers.',
  },
};

const AboutPage = async () => {
  return (
    <div className='my-0 mx-auto max-w-2xl min-h-full'>
      <AboutContent />
    </div>
  );
};

export default AboutPage;
