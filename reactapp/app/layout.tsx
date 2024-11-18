import getSongsByUserId from '@/api/getSongsByUserId';
import Sidebar from '@/components/Sidebar';
import { GoogleAnalytics } from '@next/third-parties/google'
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
import Script from 'next/script';

export const metadata = {
  title: 'Noizera',
  description:
    'Noizera - independent music streaming',
};

export const revalidate = 0;

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
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
                    <ModalProvider />
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
                        <Header />
                        {children}
                        <AudioPlayer />
                      </div>
                    </Sidebar>
                  </SongProvider>
                </LibraryProvider>
              </UserProvider>
            </AuthProvider>
          </div>
        </ThemeProvider>

      </body>
      <GoogleAnalytics gaId={process?.env?.NEXT_PUBLIC_GOOGLE_ANALYTICS ?? ""} />
      {/* Google Analytics script */}
      {/* <Script
        src={`https://www.googletagmanager.com/gtag/js?id=G-39TE4W9KX4`}
        strategy="afterInteractive"
      />
      <Script
        id="google-analytics"
        strategy="afterInteractive"
        dangerouslySetInnerHTML={{
          __html: `
            window.dataLayer = window.dataLayer || [];
            function gtag(){dataLayer.push(arguments);}
            gtag('js', new Date());
            gtag('config', 'G-39TE4W9KX4');
          `,
        }}
      /> */}
    </html>
  );
}
