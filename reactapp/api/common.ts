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
                title: "Please sign in",
                description: "Please sign in and try again",
              });
        } else if (err.response?.status === 400) {
            toast({
                variant: "destructive",
                title: "Some errors in your request",
                description: err.response.data?.detail,
            });
        } else {
            toast({
                variant: "destructive",
                title: "Oops... something went wrong",
                description: err.response.data?.detail,
              });
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