'use client';

import * as React from "react"
import { Button } from "@/components/ui/button"
import {
  EllipsisVertical,
  Shield,
  Star
} from "lucide-react"

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useRouter } from "next/navigation";

export function SettingsMenu() {
  const router = useRouter();

  return (<DropdownMenu>
    <DropdownMenuTrigger asChild>
      <Button variant='ghost' className="rounded-full" size="icon">
        <EllipsisVertical size={25} />
      </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent className="w-56 hover:cursor-pointer">
      <DropdownMenuGroup>
        <DropdownMenuItem onClick={() => router.push(`/subscriptions`)}>
          <Star className="mr-2 h-4 w-4" />
          <span>Get Noizera Premium</span>
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => router.push(`/terms`)}>
          <Shield className="mr-2 h-4 w-4" />
          <span>Terms & privacy policy</span>
        </DropdownMenuItem>
      </DropdownMenuGroup>
    </DropdownMenuContent>
  </DropdownMenu>);
}
