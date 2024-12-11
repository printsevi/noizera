'use client';

import useLocalStorage from "@/hooks/useLocalStorage";
import { decodeJwtToken, getURL } from "@/libs/helpers";
import { useRouter } from "next/navigation";
import { ReactNode, createContext, useEffect, useState } from "react";

interface Props {
    children?: ReactNode
}

export interface ISubcriptionModel {
    productIds?: string[];
}

export interface IAuthModel {
    accessToken?: string;
    refreshToken?: string;
    userId?: string;
    email?: string;
    role?: string;
}

export interface IAuthContext {
    auth: IAuthModel;
    signIn: (accessToken?: string, refreshToken?: string) => void;
    signOut: () => void;
    isAuthenticated: boolean;
}

const SESSION_KEY = 'session-auth';

const AuthContext = createContext<IAuthContext | undefined>(undefined);

const AuthContextProvider = ({ children }: Props) => {
    const [session, setSession] = useLocalStorage<IAuthModel>(SESSION_KEY, {});
    const [auth, setAuth] = useState<IAuthModel>(session);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const router = useRouter();

    const signOut = () => {
        setAuth({});
        setIsAuthenticated(false);
        setSession({});
        router.push('/');
    };

    const signIn = (accessToken?: string, refreshToken?: string) => {
        if (accessToken && refreshToken) {
            const decodedToken = decodeJwtToken(accessToken);
            setAuth({
                accessToken: accessToken,
                refreshToken: refreshToken,
                userId: decodedToken.user_id,
                email: decodedToken.email,
                role: decodedToken.role,
            });
        }
    };

    useEffect(() => {
        if (auth.accessToken && auth.refreshToken) {
            const decodedToken = decodeJwtToken(auth.accessToken);
            setSession({
                accessToken: auth.accessToken,
                refreshToken: auth.refreshToken,
                userId: decodedToken.user_id,
                email: decodedToken.email,
                role: decodedToken.role,
            });
            setIsAuthenticated(true);
        } else {
            setSession({});
        }

    }, [auth.accessToken, auth.refreshToken]);

    return (
        <AuthContext.Provider value={{ auth, signIn, signOut, isAuthenticated }}>
            {children}
        </AuthContext.Provider>
    )
}

export const AuthProvider: React.FC<Props> = ({ children }) => {
    return <AuthContextProvider>{children}</AuthContextProvider>;
};

export default AuthContext;