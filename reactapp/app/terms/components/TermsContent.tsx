'use client';

import getLatestTerms from '@/api/users/getLatestTerms';
import React, { useEffect, useState } from 'react';
import useSWR from 'swr';

const TermsContent = () => {
  const { data, isLoading } = useSWR(getLatestTerms.name, () => getLatestTerms(), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [terms, setTerms] = useState('');

  useEffect(() => {
    if(data?.ok) {
      setTerms(data.data?.content ?? "");
    }
  }, [data]);

  return (
    <div className="prose dark:prose-invert max-w-none"> {/* Use Tailwind's prose for default styling */}
      {/* <div dangerouslySetInnerHTML={{ __html: terms }} /> */}
      {terms}
    </div>
  );
};

export default TermsContent;
