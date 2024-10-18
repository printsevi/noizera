// import getSongs from '@/actions/getSongs';
// import Header from '@/components/Header';
// import ListItem from '@/components/ListItem';

import Header from "@/components/Header";
import ListItem from "@/components/ListItem";
import SearchInput from "@/components/SearchInput";
import { Card, CardContent } from "@/components/ui/card";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel";
import PageContent from "./components/PageContent";
import { usePageLoading } from "@/hooks/usePageLoading";
import Loading from "../loading";
import dynamic from "next/dynamic";

// import PageContent from './components/PageContent';

export const revalidate = 0;

export default async function Home() {
  //const songs = await getSongs();
  // const { isPageLoading } = usePageLoading();

  //return isPageLoading ? <Loading /> : <PageContent songs={[]} />;
  return (
    <div className='min-h-full px-3'>
      <PageContent songs={[]} />
    </div>
  );
}
