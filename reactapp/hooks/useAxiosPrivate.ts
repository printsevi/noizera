import { useEffect, useState } from "react";
import useAuth from "./useAuth";
import { axiosPrivate } from "@/libs/axios";
import getNewAccessToken from "@/api/auth/getNewAccessToken";
import { ErrorCodes } from "@/api/common";

const useAxiosPrivate = () => {
    const { auth, signIn, signOut } = useAuth();
    const [isReady, setIsReady] = useState(false); 
    const [isAuthorized, setIsAuthorized] = useState(false); 
    useEffect(() => {
        const requestIntercept = axiosPrivate.interceptors.request.use(
            config => {
                if (!config.headers['Authorization'] && auth?.accessToken) {
                    config.headers['Authorization'] = `Bearer ${auth?.accessToken}`;
                } 

                if (config.headers['Authorization']) {
                    setIsAuthorized(true);
                }
                
                return config;
            }, (error) => Promise.reject(error)
        );
        const responseIntercept = axiosPrivate.interceptors.response.use(
            response => response,
            async (error) => {
                const prevRequest = error?.config;
                const status = error?.response?.status;
                if ((status === 403 || status === 401) && !prevRequest?.sent) {
                    prevRequest.sent = true; 
                    const response = await getNewAccessToken(auth.refreshToken!, auth.accessToken!);
                    if (!response.ok) {
                        if (response.problem?.errorCode === ErrorCodes.RefreshTokenRevoked) {
                            signOut();
                        }
                    }

                    prevRequest.headers.Authorization = `Bearer ${response.data?.accessToken}`;
                    signIn(response.data?.accessToken, response.data?.refreshToken);
                    
                    setIsAuthorized(true);
                    
                    return axiosPrivate(prevRequest);
                }
                return Promise.reject(error);
            }
        );

        setIsReady(true);

        return () => {
            axiosPrivate.interceptors.request.eject(requestIntercept);
            axiosPrivate.interceptors.response.eject(responseIntercept);
        }
    }, [auth])

    return { axiosPrivate, isReady, isAuthorized };
}

export default useAxiosPrivate;