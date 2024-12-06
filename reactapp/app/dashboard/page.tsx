import { Metadata } from 'next';
import DashboardContent from './components/DashboardContent';

export const metadata: Metadata = {
  title: 'Release your music',
  description: 'Release your music on our independent music streaming platform',
  robots: {
    index: false,  // Do not index this page
    follow: false, // Do not follow links on this page
  }
};

const Dashboard = async () => {
  return (
    <div className='my-0 mx-auto max-w-full min-h-full'>
      <DashboardContent />
    </div>
  );
};

export default Dashboard;
