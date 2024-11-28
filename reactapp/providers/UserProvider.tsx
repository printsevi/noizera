'use client';

import { ProfileType } from "@/api/common";
import getMyUser, { GetMyUserResponse } from "@/api/users/getMyUser";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { ReactNode, createContext, useCallback, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface IUserContext {
    user?: GetMyUserResponse;
    setUser: (action: GetMyUserResponse | ((prevState: GetMyUserResponse | undefined) => GetMyUserResponse)) => void;
}

const UserContext = createContext<IUserContext | undefined>(undefined);

const UserContextProvider = ({ children }: Props) => {
    const { auth, isAuthenticated } = useAuth();
    const { isReady, axiosPrivate } = useAxiosPrivate();
    const [user, setUser] = useState<GetMyUserResponse>();

    const fetchUser = useCallback(async () => {
        if (isReady && isAuthenticated) {
            const data = await getMyUser(axiosPrivate, auth.userId!)
            if (data.ok) {
                setUser(data.data);
            }
        }
    }, [isAuthenticated, isReady, axiosPrivate, auth.userId]);

    useEffect(() => {
        if (isReady && isAuthenticated) {
            fetchUser();
        }
    }, [isReady, isAuthenticated, fetchUser]);

    // useEffect(() => {
    //     if (isReady && isAuthenticated) {
    //         fetchUser(); // Initial fetch when component mounts
    //     }

    //     const intervalId = setInterval(() => {
    //         if (isReady && isAuthenticated) {
    //             fetchUser();
    //         }
    //     }, 60000); // 1 minute = 60,000 ms

    //     // Cleanup the interval on unmount
    //     return () => clearInterval(intervalId);
    // }, [isReady, isAuthenticated, fetchUser]);

    return (
        <UserContext.Provider value={{ user, setUser }}>
            {children}
        </UserContext.Provider>
    )
}

export const UserProvider: React.FC<Props> = ({ children }) => {
    return <UserContextProvider>{children}</UserContextProvider>;
};

export default UserContext;