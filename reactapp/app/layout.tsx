import getSongsByUserId from '@/api/getSongsByUserId';
import AppSidebar from '@/components/AppSidebar';
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
import { CookieConsentProvider } from '@/providers/CookieConsentProvider';
import { CookieConsent } from '@/components/CookieConsent';
import { Metadata, Viewport } from 'next';
import Head from 'next/head';
import { Toaster } from '@/components/ui/toaster';
import { SidebarProvider } from '@/components/ui/sidebar';

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  maximumScale: 1,
  userScalable: false,
  // Example of an additional property
  interactiveWidget: 'resizes-visual',
  viewportFit: 'cover'
};

export const metadata: Metadata = {
  title: {
    default: 'Noizera',
    template: '%s | Noizera',
  },
  description: 'Discover a music streaming platform for independent artists and labels offering high royalties. Enjoy FLAC-quality songs, curated playlists, and affordable subscriptions.',
  openGraph: {
    title: 'Noizera | Independent Music Streaming',
    description: 'Discover a music streaming platform for independent artists and labels offering high royalties. Enjoy FLAC-quality songs, curated playlists, and affordable subscriptions.',
    siteName: 'Noizera',
    type: 'website',
    images: ['/images/logo.png'],
  },
  robots: {
    index: true,
    follow: true,
  }
};

export const revalidate = 0;

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang='en' className='dark' suppressHydrationWarning>
      <body
        className={`${GeistSans.variable} ${GeistMono.variable} font-sans antialiased`}
      >
        <ThemeProvider
          attribute='style'
          themes={['dark', 'system', 'light']}
          defaultTheme='system'
          enableSystem
          storageKey="theme"
          disableTransitionOnChange
        >
          <AuthProvider>
            <UserProvider>
              <CookieConsentProvider>
                <LibraryProvider>
                  <SongProvider>
                    <ModalProvider />
                    <SidebarProvider>
                      <div className='flex flex-1 overflow-hidden'>
                        <AppSidebar />
                        <div className="flex flex-col flex-1 overflow-hidden">
                          <Header />
                          <main className="flex-1 overflow-y-auto overflow-x-hidden pt-safe pb-24">
                            {children}
                          </main>
                        </div>
                      </div>
                      <AudioPlayer />
                    </SidebarProvider>
                    <Toaster />
                    <CookieConsent />
                  </SongProvider>
                </LibraryProvider>
              </CookieConsentProvider>
            </UserProvider>
          </AuthProvider>
        </ThemeProvider>
      </body>
      <Script
        src={`https://www.googletagmanager.com/gtag/js?id=${process?.env.NEXT_PUBLIC_GOOGLE_ANALYTICS ?? ""}`}
        strategy="afterInteractive"
      />
      <Script id="google-analytics" strategy="afterInteractive">
        {`
            window.dataLayer = window.dataLayer || [];
            function gtag(){dataLayer.push(arguments);}
            gtag('js', new Date());
            gtag('config', '${process?.env.NEXT_PUBLIC_GOOGLE_ANALYTICS ?? ""}', { send_page_view: false, page_path: window.location.pathname });
          `}
      </Script>
    </html>
  );
}
