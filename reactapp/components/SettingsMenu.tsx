'use client';

import * as React from "react"

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { useMediaQuery } from "@custom-react-hooks/use-media-query"
import {
  Cloud,
  CreditCard,
  Disc3Icon,
  EllipsisVertical,
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

export function SettingsMenu() {
  const router = useRouter();

  return (<DropdownMenu>
    <DropdownMenuTrigger asChild>
      <Button variant='ghost' className="rounded-full" size = "icon">
        <EllipsisVertical size={25}/>
      </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent className="w-56 hover:cursor-pointer">
      <DropdownMenuGroup>
      <DropdownMenuItem onClick={() => router.push(`/terms`)}> 
            <Shield className="mr-2 h-4 w-4" />
            <span>Terms & privacy policy</span>
          </DropdownMenuItem>
      </DropdownMenuGroup>
    </DropdownMenuContent>
  </DropdownMenu>);
}
