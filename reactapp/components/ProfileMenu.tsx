'use client';

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { useMediaQuery } from "@custom-react-hooks/use-media-query"
import {
  BadgeInfo,
  Disc3Icon,
  LayoutDashboard,
  LogOut,
  Mail,
  Settings,
  Shield,
  Star,
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
import { getProfileImageSrc, getURL } from "@/libs/helpers";

export function ProfileMenu() {
  const router = useRouter();
  const isDesktop = useMediaQuery("(min-width: 768px)");
  const { isReady, axiosPrivate } = useAxiosPrivate();
  const { auth, isAuthenticated, signOut: logOut } = useAuth();
  const { user } = useUser();

  const handleLogout = async () => {
    const response = await signOut(axiosPrivate, auth.userId!);
    if (response.ok) {
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
            <AvatarImage src={getProfileImageSrc(user?.profilePublicId)} alt={user?.username} />
            <AvatarFallback>{user?.username.slice(0, 2).toUpperCase()}</AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-56 hover:cursor-pointer">
        <DropdownMenuGroup>
          <DropdownMenuItem onClick={() => router.push(`/profiles/${user?.username.toLowerCase()}`)}>
            <User className="mr-2 h-4 w-4" />
            <span>Your page</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push(`/settings`)}>
            <Settings className="mr-2 h-4 w-4" />
            <span>Account Center</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push('/new-album')}>
            <Disc3Icon className="mr-2 h-4 w-4" />
            <span>Release Music</span>
          </DropdownMenuItem>
          {(user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) && <DropdownMenuItem onClick={() => router.push(`/dashboard`)}>
            <LayoutDashboard className="mr-2 h-4 w-4" />
            <span>Your releases</span>
          </DropdownMenuItem>}
          <DropdownMenuItem onClick={handleLogout}>
            <LogOut className="mr-2 h-4 w-4" />
            <span>Sign out</span>
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem onClick={() => router.push('/subscriptions')}>
            <Star className="mr-2 h-4 w-4" />
            <span>Subscriptions</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push(`/about`)}>
            <BadgeInfo className="mr-2 h-4 w-4" />
            <span>About us</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push(`/contact`)}>
            <Mail className="mr-2 h-4 w-4" />
            <span>Contact us</span>
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => router.push(`/terms`)}>
            <Shield className="mr-2 h-4 w-4" />
            <span>Terms & privacy policy</span>
          </DropdownMenuItem>
        </DropdownMenuGroup>
      </DropdownMenuContent>
    </DropdownMenu>);
  }
}

