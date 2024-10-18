'use client';

import getFollowersCount from "@/api/profiles/getFollowersCount";
import getProfile from "@/api/profiles/getProfile";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import useUser from "@/hooks/useUser";
import useSWR from "swr";

interface Props {
  profilePublicId: string,
}

export default function MusicCollectionContent(props: Props) {
  const { auth } = useAuth();
  const { user } = useUser();
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { data, isLoading } = useSWR(getProfile.name, () => getProfile(props.profilePublicId), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });

  if(!data?.ok) {
    return <></>;
  }

  const onFollowToggle = async () => {

  }

  return (
    <div className="pl-6 pr-6">
      <div className="flex">
      <div className="flex-initial">
        <Avatar className="w-24 h-24 md:w-36 md:h-36">
          <AvatarImage src={data.data?.profileImageSrc ?? ""} />
          <AvatarFallback>CN</AvatarFallback>
        </Avatar>
      </div>
      <div className="flex-1 pl-8">
        <div className="flex">
          <h1 className="text-xl pl-4">{props.profilePublicId}</h1>
          {user?.username !== props.profilePublicId && <Button
            variant={data.data?.isFollowing ? 'secondary' : 'default'}
            onClick={onFollowToggle}
          >
            {data.data?.isFollowing ? 'Following' : 'Follow'}
          </Button>}
        </div>
        <div className="flex">
          <Button variant="ghost" className=""><span className="font-bold pr-1 text-lg">{data?.data?.followersCount ?? 0}</span>followers</Button>
          <Button variant="ghost" className=""><span className="font-bold pr-1 text-lg">{data?.data?.followingsCount ?? 0}</span>following</Button>
        </div>
        {data.data?.name && <div className="pl-4">
          <h1 className="font-bold text-lg">{data.data?.name}</h1>
        </div>}
        {data.data?.bio && <div className="pl-4">
          <h1 className="text-sm">{data.data?.bio}</h1>
        </div>}
      </div>
      </div>
    <div className="w-full mt-8">
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 px-6">
        {/* {albums.map((album) => (
          <Card key={album.id} className="shadow-lg hover:shadow-2xl">
            <img
              className="w-full h-48 object-cover rounded-t-lg"
              src={album.coverImage}
              alt={`${album.title} cover`}
            />
            <div className="p-4">
              <h3 className="text-xl font-semibold text-gray-800">
                {album.title}
              </h3>
              <p className="text-gray-500">{album.releaseDate}</p>
              <Button className="mt-4" variant="ghost">
                <CarIcon className="w-5 h-5 mr-2" />
                Play
              </Button>
            </div>
          </Card>
        ))} */}
      </div>
    </div>
    </div>
  );
}
