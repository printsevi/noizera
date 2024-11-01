'use client';

import getLatestTerms from '@/api/users/getLatestTerms';
import React, { useEffect, useState } from 'react';
import ReactMarkdown from 'react-markdown';
import rehypeSlug from 'rehype-slug';
import rehypeAutolinkHeadings from 'rehype-autolink-headings';
import useSWR from 'swr';

const TermsContent = () => {
  const { data, isLoading } = useSWR(getLatestTerms.name, () => getLatestTerms(), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [terms, setTerms] = useState('');

  useEffect(() => {
    if (data?.ok) {
      setTerms(data.data?.content ?? "");
    }
  }, [data]);

  return (
    <div className="p-4 sm:p-6 lg:p-8 min-h-screen bg-background mx-auto w-full">
      <div className="dark:prose-invert prose prose-sm sm:prose lg:prose-lg xl:prose-xl w-full">
        <ReactMarkdown
          rehypePlugins={[rehypeSlug, rehypeAutolinkHeadings]}
        >
          {terms}
        </ReactMarkdown>
      </div>
    </div>

  );
};

export default TermsContent;
