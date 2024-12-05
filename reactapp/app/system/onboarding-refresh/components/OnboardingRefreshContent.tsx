'use client';

import startSubscription from "@/api/subscriptions/startSubscription";
import getOrCreateConnectedAccount from "@/api/users/getOrCreateConnectedAccount";
import { Progress } from "@/components/ui/progress";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useUser from "@/hooks/useUser";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import useSWR from "swr";

export default function OnboardingRefreshContent() {
  const { auth, isAuthenticated } = useAuth();
  const router = useRouter();
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { user, setUser } = useUser();
  const { data, isLoading } = useSWR(isAuthenticated && isReady ? getOrCreateConnectedAccount.name : null, () => getOrCreateConnectedAccount(axiosPrivate, auth.userId!), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [progress, setProgress] = useState(13);

  useEffect(() => {
    const timer = setTimeout(() => setProgress(66), 300)
    return () => clearTimeout(timer)
  }, [])

  useEffect(() => {
    if (data?.ok) {
      document.location.href = data.data!.url;
    }
  }, [data]);

  return <div className='h-full flex items-center justify-center'>
    <Progress value={progress} className="w-[60%] bg-purple-500" />
  </div>
}
