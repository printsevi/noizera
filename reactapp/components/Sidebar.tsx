'use client';

import { HiHome } from 'react-icons/hi';
import { MdOutlineExplore } from "react-icons/md";
import { twMerge } from 'tailwind-merge';
import { usePathname, useRouter } from 'next/navigation';

import { Song } from '@/types';
import usePlayer from '@/hooks/usePlayer';

import SidebarItem from './SidebarItem';
import Box from './Box';
import Library from './Library';
import { useMemo } from 'react';
import { Toaster } from './ui/toaster';
import { Home, HouseIcon, LibraryBigIcon, LibraryIcon, Search } from 'lucide-react';
import Header from './Header';
import { Button } from './ui/button';

interface SidebarProps {
  children: React.ReactNode;
}

const Sidebar = ({ children }: SidebarProps) => {
  const pathname = usePathname();
  const player = usePlayer();
  const router = useRouter();

  const routes = useMemo(
    () => [
      {
        icon: HouseIcon,
        label: 'Home',
        active: pathname !== '/library',
        href: '/',
      },
      {
        icon: LibraryBigIcon,
        label: 'Library',
        href: '/library',
        active: pathname === '/library',
      },
    ],
    [pathname]
  );

  return (
    <div
      className="
      bg-neutral-950        
      flex 
      rounded-lg 
      relative
      w-full"
    >
      <aside className="w-[72px] md:w-[230px]  border-r p-4 hidden md:block transition-all duration-300 ease-in-out">
        <nav className="space-y-2 sticky top-3">
          <h1 className="text-xl font-bold">Noizera</h1>
          <Button variant="ghost" className="w-full justify-start" onClick={() => router.push(`/`)}>
            <Home className="mr-2 h-4 w-4" />
            <span className="hidden md:inline">Home</span>
          </Button>
          <Button variant="ghost" className="w-full justify-start" onClick={() => router.push(`/library`)}>
            <LibraryIcon className="mr-2 h-4 w-4" />
            <span className="hidden md:inline">Library</span>
          </Button>
          <Box className='overflow-y-auto h-full'>
            <Library songs={[]} />
          </Box>
        </nav>
      </aside>
      <main className='flex-1 overflow-y-scroll overflow-x-hidden relative h-screen pb-44 md:pb-20'>
        <Header />
        {children}
      </main>

    </div>
  );
};

export default Sidebar;
