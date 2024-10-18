'use client';

import { useEffect, useState } from 'react';
import { Progress } from '@/components/ui/progress';

const Loading = () => {
  const [progress, setProgress] = useState(13)
 
  useEffect(() => {
    const timer = setTimeout(() => setProgress(66), 500)
    return () => clearTimeout(timer)
  }, [])
 
  return <div className='h-full flex items-center justify-center'>
    <Progress value={progress} className="w-[60%]" />
  </div>
};

export default Loading;
