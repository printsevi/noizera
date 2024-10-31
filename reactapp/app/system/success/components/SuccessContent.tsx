'use client';

import startSubscription from "@/api/subscriptions/startSubscription";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useUser from "@/hooks/useUser";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import useSWR from "swr";

interface Props {
  sessionId: string;
}

export default function SuccessContent({ sessionId }: Props) {
  const { auth, isAuthenticated } = useAuth();
  const router = useRouter();
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { user, setUser } = useUser();
  const { data, isLoading } = useSWR(isAuthenticated && isReady && sessionId ? startSubscription.name : null, () => startSubscription(axiosPrivate, auth.userId!, sessionId as string), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });

  useEffect(() => {
    if (data?.ok) {
      // setUser(prev => ({...prev!, activeSubscriptions: [...prev.activeSubscriptions, data.data?.activeSubscriptionType]}))
      console.log(data.data?.activeSubscriptionType);
      router.push('/')
    }
  }, [data]);

  return (<></>);
}
