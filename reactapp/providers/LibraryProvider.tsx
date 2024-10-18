'use client';

import getMyUser from "@/api/users/getMyUser";
import useAuth from "@/hooks/useAuth";
import { ReactNode, createContext, useEffect, useState } from "react";
import useSWR from "swr";

interface Props {
    children?: ReactNode
}

export interface IMusicCollectionModel {
    collectionType: string;
    title: string;
    publicId: string;
}

export interface ILibraryContext {
    addCollection: (newCollection: IMusicCollectionModel) => void;
    collections: IMusicCollectionModel[];
}

const LibraryContext = createContext<ILibraryContext | undefined>(undefined);

const LibraryContextProvider = ({ children }: Props) => {
    // const { axiosPrivate, isReady } = useAxiosPrivate();
    // const { auth } = useAuth();
    // const { data, isLoading } = useSWR(isReady && auth.accessToken ? getMyUser.name : null, () => getMyUser(axiosPrivate, auth.userId!), {
    //     revalidateIfStale: true,
    //     revalidateOnFocus: false,
    //     revalidateOnReconnect: false});
     const [collections, setCollections] = useState<IMusicCollectionModel[]>([]);

    // useEffect(() => {
    //     if (isLoading) {
    //         return;
    //     }

    //     if (data?.ok && data.data) {
    //         setCollections([]);
    //     }
        
    // }, [data, isLoading]);

    const addCollection = (newCollection: IMusicCollectionModel) => {

    }

    return (
        <LibraryContext.Provider value={{ collections, addCollection }}>
            {children}
        </LibraryContext.Provider>
    )
}

export const LibraryProvider: React.FC<Props> = ({ children }) => {
  return <LibraryContextProvider>{children}</LibraryContextProvider>;
};

export default LibraryContext;