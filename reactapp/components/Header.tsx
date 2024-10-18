'use client';

import { useRouter } from 'next/navigation';

import useSignInModal from '@/hooks/useSignInModal';
import useAuth from '@/hooks/useAuth';
import usePlayer from '@/hooks/usePlayer';

import PurpleButton from './Button';
import useSignUpModal from '@/hooks/useSignUpModal';
import { useEffect, useState } from 'react';
import { Button } from './ui/button';
import { ProfileMenu } from './ProfileMenu';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle, SheetTrigger } from './ui/sheet';
import { AlignJustify, Home, Library, Search, X } from 'lucide-react';
import { Input } from './ui/input';
import { ScrollArea } from './ui/scroll-area';
import { cn } from '@/lib/utils';
import { SettingsMenu } from './SettingsMenu';

const Header: React.FC = () => {
  const router = useRouter();
  const signInModal = useSignInModal();
  const signUpModal = useSignUpModal();
  const { isAuthenticated } = useAuth();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [showMobileSearch, setShowMobileSearch] = useState(false);
  const [isSearching, setIsSearching] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");

  const [isClient, setIsClient] = useState(false)

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value
    setSearchQuery(value)
    setIsSearching(value.length > 0)
  }

  const handleSearchBlur = () => {
    if (searchQuery === "") {
      setIsSearching(false)
    }
  }
 
  useEffect(() => {
    setIsClient(true)
  }, []);

  return (
    <header className="flex items-center justify-between p-3 relative z-10 sticky top-0 bg-background bg-opacity-50">
        <div className="flex items-center space-x-4">
          <Sheet>
            <SheetTrigger asChild>
              <Button variant="ghost" size="icon" className="md:hidden">
                <AlignJustify className="h-6 w-6" />
              </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-[300px] sm:w-[400px]">
              <SheetHeader>
                <SheetTitle>Menu</SheetTitle>
                <SheetDescription>
                </SheetDescription>
              </SheetHeader>
              <nav className="flex flex-col space-y-4">
                <Button variant="ghost" className="justify-start">
                  <Home className="mr-2 h-4 w-4" />
                  Home
                </Button>
                <Button variant="ghost" className="justify-start">
                  <Library className="mr-2 h-4 w-4" />
                  Library
                </Button>
              </nav>
            </SheetContent>
          </Sheet>
        </div>
        <div className={cn("md:flex-1 md:max-w-xl md:relative md:p-0 flex items-center p-4 ", showMobileSearch ? "bg-background inset-0 absolute" : "")}>
          <Input
            type="search"
            placeholder="Search songs, artists, albums"
            className={cn("w-full md:block", showMobileSearch ? "z-50" : "hidden")}
            value={searchQuery}
            onChange={handleSearchChange}
            onBlur={handleSearchBlur}
          />
          <Button
              variant="ghost"
              size="icon"
              className={cn("ml-2 md:hidden", showMobileSearch ? "" : "hidden")}
              onClick={() => setShowMobileSearch(false)}
            >
              <X className="h-4 w-4" />
            </Button>
            {/* <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            onClick={() => setShowMobileSearch(true)}
          >
            <Search className="h-6 w-6" />
          </Button> */}
          {isSearching && (
            <div className="absolute top-full left-0 right-0 bg-popover border rounded-md mt-1 shadow-lg z-10">
              <ScrollArea className="h-[300px]">
                {/* {mockSearchResults.map((result) => (
                  <Button key={result.id} variant="ghost" className="w-full justify-start px-4 py-2">
                    <div>
                      <div className="font-medium">{result.title}</div>
                      <div className="text-sm text-muted-foreground">{result.artist || result.description || result.type}</div>
                    </div>
                  </Button>
                ))} */}
              </ScrollArea>
            </div>
          )}
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            onClick={() => setShowMobileSearch(true)}
          >
            <Search className="h-6 w-6" />
          </Button>
          {isClient && isReady && isAuthenticated && (
            <>
              <div>
                <PurpleButton onClick={() => router.push('/subscriptions')} className='bg-white px-6 py-2'>
                  Start free trial
                </PurpleButton>
              </div>
              <div className={cn('md:block', showMobileSearch ? "hidden" : "")}>
                <ProfileMenu/>
              </div>
            </>
          )}
          {isClient && isReady && !isAuthenticated && (
            <>
              <div>
                <Button
                  onClick={signUpModal.onOpen}
                  variant='ghost'
                  className='
                    rounded-full
                    hidden
                    md:block
                  '
                >
                  Sign up
                </Button>
              </div>
              <div>
                <PurpleButton
                  onClick={signInModal.onOpen}
                  className='bg-white px-6 py-2'
                >
                  Sign in
                </PurpleButton>
              </div>
              <div className={cn('md:block', showMobileSearch ? "hidden" : "")}>
                <SettingsMenu/>
              </div>
            </>
          )}
        </div>
    </header>
  );
};

export default Header;
