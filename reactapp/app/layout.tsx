import getSongsByUserId from '@/api/getSongsByUserId';
import Sidebar from '@/components/Sidebar';
import './globals.css';
import AudioPlayer from '@/components/AudioPlayer';
import { AuthProvider } from '@/providers/AuthProvider';
import ModalProvider from '@/providers/ModalProvider';
import Header from '@/components/Header';
import { GeistSans } from 'geist/font/sans'
import { GeistMono } from 'geist/font/mono'
import { SongProvider } from '@/providers/SongProvider';
import { UserProvider } from '@/providers/UserProvider';
import { LibraryProvider } from '@/providers/LibraryProvider';
import { ThemeProvider } from '@/providers/ThemeProvider';

export const metadata = {
  title: 'Noizera',
  description:
    'Noizera - independent music streaming',
};

export const revalidate = 0;

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const userSongs = await getSongsByUserId();

  return (
    <html lang='en' className='dark' suppressHydrationWarning>
      <body className={`${GeistSans.variable} ${GeistMono.variable} font-sans`}>
        <ThemeProvider
            attribute='style'
            themes={['dark', 'system', 'light']}
            defaultTheme='system'
            enableSystem
            storageKey="theme"
            disableTransitionOnChange
          >
        <div className="h-screen flex flex-col h-screen dark:bg-black">
          <AuthProvider>
            <UserProvider>
              <LibraryProvider>
              <SongProvider>
                <ModalProvider/>
                <Sidebar>
                  <div
                    className='
                      rounded-lg 
                      h-full 
                      w-full 
                      overflow-hidden 
                      overflow-y-auto
                    '
                  >
                    <Header/>
                    {children}
                    <AudioPlayer/>
                  </div>
                </Sidebar>
              </SongProvider>
              </LibraryProvider>
            </UserProvider>
          </AuthProvider>
        </div>
        </ThemeProvider>
        
      </body>
    </html>
  );
}
