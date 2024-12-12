import { Metadata } from 'next';
import SettingsContent from './components/SettingsContent';

export const metadata: Metadata = {
  title: 'Account center',
  description: 'Manage your account settings and preferences.',
  robots: {
    index: false,  // Do not index this page
    follow: false, // Do not follow links on this page
  }
};

const SettingsPage = async () => {
  return (
    <div className='my-0 mx-auto max-w-xl min-h-full'>
      <SettingsContent />
    </div>
  );
};

export default SettingsPage;
