'use client';

import checkout from "@/api/subscriptions/checkout";
import getSubscriptions from "@/api/subscriptions/getSubscriptions";
import startSubscription from "@/api/subscriptions/startSubscription";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { CheckIcon, MinusIcon, ReceiptEuro } from "lucide-react";
import { useRouter, useSearchParams  } from "next/navigation";
import { useEffect, useState } from "react";
import useSWR from "swr";

export default function SuccessContent() {
  const { auth } = useAuth();
  const router = useRouter();
  const searchParams  = useSearchParams();
  const sessionId = searchParams.get('session_id');
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { data, isLoading } = useSWR(isReady && sessionId ? startSubscription.name : null, () => startSubscription(axiosPrivate, auth.userId!, sessionId as string), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });

  useEffect(() => {
    if (data?.ok) {
      console.log(data.data?.activeSubscriptionType);
      router.push('/')
    }
  }, [data]);

  return (<></>);
}
