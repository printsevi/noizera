'use client';

import * as React from "react"

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerDescription,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
} from "@/components/ui/drawer"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useMediaQuery } from "@custom-react-hooks/use-media-query"
import {
  Cloud,
  CreditCard,
  Disc3Icon,
  Github,
  Keyboard,
  LifeBuoy,
  LogOut,
  Mail,
  MessageSquare,
  Music,
  Plus,
  PlusCircle,
  Settings,
  Shield,
  User,
  UserPlus,
  Users,
} from "lucide-react"

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuPortal,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useRouter } from "next/navigation";
import signOut from "@/api/auth/signOut";
import useAuth from "@/hooks/useAuth";
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar";
import useUser from "@/hooks/useUser";
import { ProfileType } from "@/api/common";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { getURL } from "@/libs/helpers";

export function ProfileMenu() {
  const router = useRouter();
  const isDesktop = useMediaQuery("(min-width: 768px)");
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const { auth, isAuthenticated, signOut: logOut } = useAuth();
  const { user } = useUser();

  const handleLogout = async () => {
    if (await signOut(axiosPrivate, auth.userId!)) {
      logOut();
    }
    //player.reset();
    router.push('/');
  };

  if (!isAuthenticated || !isReady) {
    return <></>;
  }

  if (true) {
    return (<DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button size="icon" variant="ghost" className="relativen rounded-full">
          <Avatar className="h-10 w-10">
            <AvatarImage src={`${getURL()}api/profiles/${user?.username}/image`} alt={user?.username} />
            <AvatarFallback>{user?.username.slice(0, 2).toUpperCase()}</AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-56 hover:cursor-pointer">
        <DropdownMenuGroup>
          <DropdownMenuItem onClick={() => router.push(`/profiles/${user?.username.toLowerCase()}`)}>
            <User className="mr-2 h-4 w-4" />
            <span>Your profile</span>
          </DropdownMenuItem>
          {user?.activeSubscriptions.length! > 0 && <DropdownMenuItem>
            <CreditCard className="mr-2 h-4 w-4" />
            <span>My Subscriptions</span>
          </DropdownMenuItem>}
          <DropdownMenuItem onClick={() => router.push(`/settings`)}>
            <Settings className="mr-2 h-4 w-4" />
            <span>Settings</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push('/new-album')}>
            <Disc3Icon className="mr-2 h-4 w-4" />
            <span>Upload music</span>
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuItem onClick={handleLogout}>
          <LogOut className="mr-2 h-4 w-4" />
          <span>Sign out</span>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={() => router.push(`/terms`)}>
          <Shield className="mr-2 h-4 w-4" />
          <span>Terms & privacy policy</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>);
  }
}


