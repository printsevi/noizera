'use client';

import startSubscription from "@/api/subscriptions/startSubscription";
import { Progress } from "@/components/ui/progress";
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
  const { data, isLoading } = useSWR(isAuthenticated && isReady && sessionId && user ? startSubscription.name : null, () => startSubscription(axiosPrivate, auth.userId!, sessionId as string), {
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
      setUser(prev => ({
        ...prev!,
        activeSubscriptions: prev?.activeSubscriptions ? [...prev.activeSubscriptions, data.data!.activeSubscriptionType] : [data.data!.activeSubscriptionType]
      }))
      router.push('/')
    }
  }, [data]);

  return <div className='h-full flex items-center justify-center'>
    <Progress value={progress} className="w-[60%] bg-purple-500" />
  </div>
}
