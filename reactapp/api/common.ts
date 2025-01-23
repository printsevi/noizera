import { toast } from "@/hooks/use-toast";
import axios from "axios";

export enum ErrorCodes {
    Common = 'Common',
    RefreshTokenRevoked = 'RefreshTokenRevoked',
};

export enum ProfileType {
    Artist = 'Artist',
    Fan = 'Fan',
    Label = 'Label'
};

export enum CollectionType {
    Album = 'collection_album',
    Playlist = 'collection_playlist'
};

export enum SubscriptionType {
    PremiumListeningWithFreeTrial = 'PremiumListeningWithFreeTrial'
};

export interface ApiResponse<T> {
    ok: boolean,
    data?: T,
    problem?: Problem
}

export interface Problem {
    errorCode: string
}

export interface IdResponse {
    value: string;
}

export interface UrlResponse {
    url: string;
}

export function handleErrorAndReturnProblem(err: any) {
    const problem: Problem = { errorCode: ErrorCodes.Common };
    if (axios.isAxiosError(err)) {
        if (!err?.response) {
            toast({
                variant: "destructive",
                title: "Oops... server is sleeping",
                description: "Please try again in a while",
            });
        } else if (err.response?.status === 401) {
            toast({
                variant: "destructive",
                title: "Please log in",
                description: "Please log in and try again",
            });
        } else if (err.response?.status === 400) {
            toast({
                variant: "destructive",
                title: err.response.data?.detail
            });
        } else {
            if (err.response?.status === 403) {
                toast({
                    variant: "destructive",
                    title: "Please log in",
                    description: "Your session is expired",
                });
            } else {
                toast({
                    variant: "destructive",
                    title: "Oops... something went wrong",
                    description: "Please try again",
                });
            }

            const errorCode = err.response.data?.errorCode as string;
            if (errorCode) {
                problem.errorCode = errorCode;
            }
        }
    } else {
        toast({
            variant: "destructive",
            title: "Oops... something went wrong"
        });
    }

    return problem;
}