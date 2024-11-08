'use client';

import getFollowersCount from "@/api/profiles/getFollowersCount";
import getProfile from "@/api/profiles/getProfile";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import useAuth from "@/hooks/useAuth";
import useUser from "@/hooks/useUser";
import { Heart, PlayCircle, Share2 } from "lucide-react";
import useSWR from "swr";

interface Props {
  profilePublicId: string,
}

export default function ProfileContent(props: Props) {
  const { auth } = useAuth();
  const { user } = useUser();
  const { data, isLoading } = useSWR(getProfile.name, () => getProfile(props.profilePublicId), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });

  if (!data?.ok) {
    return <></>;
  }

  const onFollowToggle = async () => {

  }

  const handleFollow = () => {
    // setIsFollowing(!isFollowing)
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex flex-col md:flex-row items-center md:items-start mb-8">
        <Avatar className="w-32 h-32 md:w-48 md:h-48 mb-4 md:mb-0 md:mr-8">
          <AvatarImage src={data.data?.profileImageSrc ?? ""} alt={data.data?.name} />
          <AvatarFallback>{data.data!.name!.slice(0, 2).toUpperCase()}</AvatarFallback>
        </Avatar>
        <div className="text-center md:text-left flex-grow">
          <h1 className="text-3xl font-bold mb-2">{data.data?.name}</h1>
          <p className="text-xl text-muted-foreground mb-4">@{props.profilePublicId}</p>
          {data.data?.bio && <p className="mb-4 max-w-md">{data.data?.bio}</p>}
          {/* <div className="flex flex-wrap justify-center md:justify-start gap-4 mb-4">
            <div>
              <span className="font-semibold">{data?.data?.followersCount ?? 0}</span> followers
            </div>
            <div>
              <span className="font-semibold">{data?.data?.followingsCount ?? 0}</span> monthly listeners
            </div>
          </div>
          <div className="flex flex-wrap justify-center md:justify-start gap-2">
            <Button className="flex-1 md:flex-none">
              <PlayCircle className="mr-2 h-4 w-4" /> Play All
            </Button>
            {user?.username !== props.profilePublicId && <Button
              variant={data.data?.isFollowing ? "default" : "outline"}
              className="flex-1 md:flex-none"
              onClick={handleFollow}
            >
              <Heart className="mr-2 h-4 w-4" /> {data.data?.isFollowing ? 'Following' : 'Follow'}
            </Button>}
            <Button variant="outline" size="icon">
              <Share2 className="h-4 w-4" />
            </Button>
          </div> */}
        </div>
      </div>

      <h2 className="text-2xl font-bold mb-4">Released Music</h2>
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
        {/* {albums.map((album) => (
          <Card key={album.id} className="overflow-hidden">
            <CardContent className="p-0">
              <div className="relative aspect-square">
                <Image
                  src={album.imageUrl}
                  alt={album.title}
                  layout="fill"
                  objectFit="cover"
                />
              </div>
            </CardContent>
            <CardFooter className="flex flex-col items-start p-2">
              <h3 className="font-semibold text-sm truncate w-full">{album.title}</h3>
              <p className="text-xs text-muted-foreground">{album.year} • {album.tracks} tracks</p>
            </CardFooter>
          </Card>
        ))} */}
      </div>
    </div>
    // <div className="pl-6 pr-6">
    //   <div className="flex">
    //   <div className="flex-initial">
    //     <Avatar className="w-24 h-24 md:w-36 md:h-36">
    //       <AvatarImage src={data.data?.profileImageSrc ?? ""} />
    //       <AvatarFallback>CN</AvatarFallback>
    //     </Avatar>
    //   </div>
    //   <div className="flex-1 pl-8">
    //     <div className="flex">
    //       <h1 className="text-xl pl-4">{props.profilePublicId}</h1>
    //       {user?.username !== props.profilePublicId && <Button
    //         variant={data.data?.isFollowing ? 'secondary' : 'default'}
    //         onClick={onFollowToggle}
    //       >
    //         {data.data?.isFollowing ? 'Following' : 'Follow'}
    //       </Button>}
    //     </div>
    //     <div className="flex">
    //       <Button variant="ghost" className=""><span className="font-bold pr-1 text-lg">{data?.data?.followersCount ?? 0}</span>followers</Button>
    //       <Button variant="ghost" className=""><span className="font-bold pr-1 text-lg">{data?.data?.followingsCount ?? 0}</span>following</Button>
    //     </div>
    //     {data.data?.name && <div className="pl-4">
    //       <h1 className="font-bold text-lg">{data.data?.name}</h1>
    //     </div>}
    //     {data.data?.bio && <div className="pl-4">
    //       <h1 className="text-sm">{data.data?.bio}</h1>
    //     </div>}
    //   </div>
    //   </div>
    // <div className="w-full mt-8">
    //   <div className="grid grid-cols-1 md:grid-cols-3 gap-6 px-6">
    //     {/* {albums.map((album) => (
    //       <Card key={album.id} className="shadow-lg hover:shadow-2xl">
    //         <img
    //           className="w-full h-48 object-cover rounded-t-lg"
    //           src={album.coverImage}
    //           alt={`${album.title} cover`}
    //         />
    //         <div className="p-4">
    //           <h3 className="text-xl font-semibold text-gray-800">
    //             {album.title}
    //           </h3>
    //           <p className="text-gray-500">{album.releaseDate}</p>
    //           <Button className="mt-4" variant="ghost">
    //             <CarIcon className="w-5 h-5 mr-2" />
    //             Play
    //           </Button>
    //         </div>
    //       </Card>
    //     ))} */}
    //   </div>
    // </div>
    // </div>
  );
}
