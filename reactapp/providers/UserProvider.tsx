'use client';

import { ProfileType } from "@/api/common";
import getMyUser from "@/api/users/getMyUser";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { ReactNode, createContext, useCallback, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface IUserModel {
    profileType: ProfileType;
    username: string;
    name: string;
    activeSubscriptions: string[];
    songCount: number;
}

export interface IUserContext {
    user?: IUserModel;
    setUser: (action: IUserModel | ((prevState: IUserModel | undefined) => IUserModel)) => void;
}

const UserContext = createContext<IUserContext | undefined>(undefined);

const UserContextProvider = ({ children }: Props) => {
    const { auth, isAuthenticated } = useAuth();
    const { isReady, axiosPrivate } = useAxiosPrivate();
    const [user, setUser] = useState<IUserModel>();

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