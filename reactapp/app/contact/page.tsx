import { Metadata } from "next";
import ContactContent from "./components/ContactContent";

export const metadata: Metadata = {
  title: 'Contact us',
  description: 'Have questions or need support? Contact us for inquiries about our music streaming platform, artist partnerships, or subscription services.',
  openGraph: {
    title: 'Contact us | Noizera',
    description: 'Have questions or need support? Contact us for inquiries about our music streaming platform, artist partnerships, or subscription services.',
  },
};

const ContactPage = async () => {
  return (
    <div className='my-0 mx-auto max-w-2xl min-h-full relative'>
      <ContactContent />
    </div>
  );
};

export default ContactPage;
