import { useEffect, useState } from "react";
import useAuth from "./useAuth";
import { axiosPrivate } from "@/libs/axios";
import getNewAccessToken from "@/api/auth/getNewAccessToken";
import { ErrorCodes } from "@/api/common";
import axios from "axios";

let isRefreshing = false;
let failedRequestsQueue: { resolve: (value: unknown) => void; reject: (reason?: any) => void; }[] = [];

const useAxiosPrivate = () => {
    const { auth, signIn, signOut } = useAuth();
    const [isReady, setIsReady] = useState(false);
    const [isAuthorized, setIsAuthorized] = useState(false);

    useEffect(() => {
        const requestIntercept = axiosPrivate.interceptors.request.use(
            (config) => {
                if (!config.headers["Authorization"] && auth?.accessToken) {
                    config.headers["Authorization"] = `Bearer ${auth?.accessToken}`;
                }

                if (config.headers["Authorization"]) {
                    setIsAuthorized(true);
                }

                return config;
            },
            (error) => Promise.reject(error)
        );

        const responseIntercept = axiosPrivate.interceptors.response.use(
            (response) => response,
            async (error) => {
                const prevRequest = error?.config;
                const status = error?.response?.status;

                if ((status === 403 || status === 401) && !prevRequest?.sent) {
                    if (isRefreshing) {
                        return new Promise((resolve, reject) => {
                            failedRequestsQueue.push({
                                resolve,
                                reject,
                            });
                        })
                            .then((token) => {
                                prevRequest.headers["Authorization"] = `Bearer ${token}`;
                                return axiosPrivate(prevRequest);
                            })
                            .catch((err) => Promise.reject(err));
                    }

                    prevRequest.sent = true;
                    isRefreshing = true;

                    try {
                        const response = await getNewAccessToken(
                            auth.refreshToken!,
                            auth.accessToken!
                        );

                        if (!response.ok) {
                            if (
                                response.problem?.errorCode === ErrorCodes.RefreshTokenRevoked
                            ) {
                                signOut();
                            }
                        }

                        signIn(response.data!.accessToken, response.data!.refreshToken);

                        failedRequestsQueue.forEach((req) =>
                            req.resolve(response.data!.accessToken)
                        );
                        failedRequestsQueue = [];

                        prevRequest.headers["Authorization"] = `Bearer ${response.data!.accessToken}`;
                        return axiosPrivate(prevRequest);
                    } catch (err) {
                        failedRequestsQueue.forEach((req) => req.reject(err));
                        failedRequestsQueue = [];
                        signOut();
                        return Promise.reject(err);
                    } finally {
                        isRefreshing = false;
                    }
                }

                return Promise.reject(error);
            }
        );

        setIsReady(true);

        return () => {
            axiosPrivate.interceptors.request.eject(requestIntercept);
            axiosPrivate.interceptors.response.eject(responseIntercept);
        };
    }, [auth]);

    return { axiosPrivate, isReady, isAuthorized };
};

export default useAxiosPrivate;
