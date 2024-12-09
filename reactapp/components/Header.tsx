'use client';

import { useRouter } from 'next/navigation';

import useSignInModal from '@/hooks/useSignInModal';
import useAuth from '@/hooks/useAuth';

import PurpleButton from './Button';
import useSignUpModal from '@/hooks/useSignUpModal';
import { useCallback, useEffect, useState } from 'react';
import { Button } from './ui/button';
import { ProfileMenu } from './ProfileMenu';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle, SheetTrigger } from './ui/sheet';
import { AlignJustify, Home, Library, Search, X } from 'lucide-react';
import { Input } from './ui/input';
import { ScrollArea } from './ui/scroll-area';
import { cn } from '@/lib/utils';
import { SettingsMenu } from './SettingsMenu';
import fastSearchPublic from '@/api/search/fastSearchPublic';
import { sendEvent } from '@/libs/helpers';
import useUser from '@/hooks/useUser';
import { useMediaQuery } from '@custom-react-hooks/use-media-query';

const Header: React.FC = () => {
  const router = useRouter();
  const signInModal = useSignInModal();
  const signUpModal = useSignUpModal();
  const { isAuthenticated } = useAuth();
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const [showMobileSearch, setShowMobileSearch] = useState(false);
  const [isSearching, setIsSearching] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState<string[]>([]);
  const [isClient, setIsClient] = useState(false)
  const user = useUser();
  const [isScrolled, setIsScrolled] = useState(false)
  const [lastScrollY, setLastScrollY] = useState(0)
  const isDesktop = useMediaQuery("(min-width: 768px)");

  // const onScroll = useCallback(() => {
  //   console.log(isDesktop);
  //   if (isDesktop) return;
  //   console.log(window.scrollY);
  //   const currentScrollY = window.scrollY;

  //   if (currentScrollY > lastScrollY) {
  //     setIsScrolled(true)
  //   } else {
  //     setIsScrolled(false)
  //   }
  //   setLastScrollY(currentScrollY)
  // }, [setIsScrolled, setLastScrollY, lastScrollY, isDesktop]);

  // useEffect(() => {
  //   window.addEventListener('scroll', onScroll) //get scrollable container because window returns default
  //   return () => window.removeEventListener('scroll', onScroll)
  // }, [onScroll])

  const [isMobile, setIsMobile] = useState(false);
  const [isInputFocused, setIsInputFocused] = useState(false);

  useEffect(() => {
    // Function to check if the device is mobile
    const checkIfMobile = () => {
      setIsMobile(window.innerWidth <= 768); // Tailwind's `md` breakpoint
    };

    const handleFocusIn = () => {
      setIsInputFocused(true);
    };

    const handleFocusOut = () => {
      setIsInputFocused(false);
    };

    checkIfMobile(); // Initial check
    window.addEventListener("resize", checkIfMobile); // Listen for resize events
    document.addEventListener("focusin", handleFocusIn); // Listen for input focus
    document.addEventListener("focusout", handleFocusOut); // Listen for input blur

    return () => {
      window.removeEventListener("resize", checkIfMobile);
      document.removeEventListener("focusin", handleFocusIn);
      document.removeEventListener("focusout", handleFocusOut);
    };
  }, []);

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

  const onSearch = useCallback(async (query: string) => {
    sendEvent({
      action: "fast_search",
      category: "interaction",
      label: "Fast search started",
      value: query
    });
    const response = await fastSearchPublic(query);
    if (response.ok) {
      setSearchResults(response.data?.map(x => x.value) ?? [])
    }
  }, [])

  const onFullSearch = useCallback(async (query: string) => {
    setSearchQuery("");
    setIsSearching(false);
    router.push(`/search?query=${query}`);
  }, [router, setSearchQuery, setIsSearching])

  useEffect(() => {
    if (!searchQuery.length) {
      return;
    }
    const timer = setTimeout(() => {
      onSearch(searchQuery)
    }, 400)
    return () => clearTimeout(timer)
  }, [searchQuery, onSearch])

  return (
    <header className={`flex w-full items-center justify-between p-2.5 z-10 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 transition-all duration-300
      ${isMobile && isInputFocused ? "!fixed" : isMobile ? "!fixed" : "sticky top-0"}
    `}
    >
      <div className="md:hidden block flex items-center space-x-4">
        <Sheet>
          <SheetTrigger asChild>
            <Button variant="ghost" size="icon" className="md:hidden">
              <AlignJustify className="h-6 w-6" />
            </Button>
          </SheetTrigger>
          <SheetContent side="left" className="w-[300px] md:w-[400px]">
            <SheetHeader>
              <SheetTitle>Menu</SheetTitle>
              <SheetDescription>
              </SheetDescription>
            </SheetHeader>
            <nav className="flex flex-col space-y-4">
              <Button variant="ghost" className="justify-start" onClick={() => router.push(`/`)}>
                <Home className="mr-2 h-4 w-4" />
                Home
              </Button>
              <Button variant="ghost" className="justify-start" onClick={() => router.push(`/library`)}>
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
          placeholder="Search songs, albums, artists, labels"
          className={cn("w-full md:block", showMobileSearch ? "z-50" : "hidden")}
          value={searchQuery}
          onChange={handleSearchChange}
          onBlur={handleSearchBlur}
        />
        <Button
          variant="ghost"
          size="icon"
          className={cn("ml-2 md:hidden", showMobileSearch ? "" : "hidden")}
          onClick={() => {
            setShowMobileSearch(false);
            setSearchQuery("");
            setIsSearching(false);
          }}
        >
          <X className="h-4 w-4" />
        </Button>
        {isSearching && searchResults.length > 0 && (
          <div className="absolute md:top-full md:left-0 md:right-0 top-16 left-3.5 right-16 bg-popover border rounded-md mt-1 shadow-lg z-10">
            <ScrollArea className="h-[300px]">
              {searchResults.map((result, index) => (
                <Button key={index} variant="ghost" onClick={() => onFullSearch(result)} className="items-center flex w-full justify-start px-4 py-2">
                  <Search className="h-4 w-4 mr-2" />
                  <span>{result}</span>
                </Button>
              ))}
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
            {!user.user?.activeSubscriptions || user.user.activeSubscriptions.length === 0 && <div>
              <PurpleButton onClick={() => router.push('/subscriptions')} className='px-6 py-2'>
                Start free trial
              </PurpleButton>
            </div>}
            <div className={cn('md:block', showMobileSearch ? "hidden" : "")}>
              <ProfileMenu />
            </div>
          </>
        )}
        {isClient && isReady && !isAuthenticated && (
          <>
            <div>
              <Button
                onClick={(e) => {
                  signUpModal.onOpen();
                }}
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
                className='px-6 py-2'
              >
                Sign in
              </PurpleButton>
            </div>
            <div className={cn('md:block', showMobileSearch ? "hidden" : "")}>
              <SettingsMenu />
            </div>
          </>
        )}
      </div>
    </header>
  );
};

export default Header;
