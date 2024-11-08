'use client';

import { CollectionType } from "@/api/common";
import getSavedMusicCollections from "@/api/savedMusicCollections/getSavedMusicCollections";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { ReactNode, createContext, useCallback, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface IMusicCollectionModel {
    collectionType: CollectionType;
    title: string;
    publicId: string;
    ownerName: string,
    ownerPublicId: string,
    songCount: number
}

export interface ILibraryContext {
    addCollection: (newCollection: IMusicCollectionModel) => void;
    removeCollection: (publicId: string) => void;
    collections: IMusicCollectionModel[];
}

const LibraryContext = createContext<ILibraryContext | undefined>(undefined);

const LibraryContextProvider = ({ children }: Props) => {
    const { axiosPrivate, isReady } = useAxiosPrivate();
    const { auth, isAuthenticated } = useAuth();
    const [collections, setCollections] = useState<IMusicCollectionModel[]>([]);

    const fetchCollections = useCallback(async () => {
        const data = await getSavedMusicCollections(auth.userId!, axiosPrivate)
        if (data.ok) {
            setCollections(data.data!);
        }
    }, [isAuthenticated, isReady, axiosPrivate, auth.userId]);

    useEffect(() => {
        if (isReady && isAuthenticated) {
            fetchCollections();
        }
    }, [isReady, isAuthenticated, fetchCollections]);

    const addCollection = (newCollection: IMusicCollectionModel) => {
        setCollections(prevItems => [...prevItems, newCollection]);
    }

    const removeCollection = (publicId: string) => {
        setCollections(prevItems => prevItems.filter(x => x.publicId !== publicId));
    }

    return (
        <LibraryContext.Provider value={{ collections, addCollection, removeCollection }}>
            {children}
        </LibraryContext.Provider>
    )
}

export const LibraryProvider: React.FC<Props> = ({ children }) => {
    return <LibraryContextProvider>{children}</LibraryContextProvider>;
};

export default LibraryContext;