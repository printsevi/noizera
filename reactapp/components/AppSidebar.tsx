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
import { Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuButton, SidebarMenuItem } from './ui/sidebar';
import Link from 'next/link';

const items = [
  {
    title: "Home",
    url: "/",
    icon: Home,
  },
  {
    title: "Library",
    url: "/library",
    icon: LibraryIcon,
  }
]

const AppSidebar = () => {
  const pathname = usePathname();
  const player = usePlayer();
  const router = useRouter();

  return (<Sidebar>
    <SidebarHeader />
    <SidebarContent>
      <SidebarGroup>
        {/* <SidebarGroupLabel>Application</SidebarGroupLabel> */}
        <SidebarGroupContent>
          <SidebarMenu>
            {items.map((item) => (
              <SidebarMenuItem key={item.title}>
                <SidebarMenuButton asChild>
                  <Link href={item.url}>
                    <item.icon />
                    <span>{item.title}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </SidebarGroupContent>
      </SidebarGroup>
    </SidebarContent>
    <SidebarFooter />
  </Sidebar>);

  return (
    <aside className="md:flex w-[230px] h-full border-r p-4 hidden flex-col flex-shrink-0 transition-all duration-300 ease-in-out">
      <div className="p-4">
        <h1 className="text-xl font-bold mb-4">Noizera</h1>
        <nav className="space-y-2">
          <Button variant="ghost" className="w-full justify-start" onClick={() => router.push('/')}>
            <Home className="mr-2 h-4 w-4" />
            <span className="hidden md:inline">Home</span>
          </Button>
          <Button variant="ghost" className="w-full justify-start" onClick={() => router.push('/library')}>
            <LibraryIcon className="mr-2 h-4 w-4" />
            <span className="hidden md:inline">Library</span>
          </Button>
        </nav>
      </div>
      <Box className='overflow-y-auto h-full'>
        <Library songs={[]} />
      </Box>
    </aside>
  );
};

export default AppSidebar;
