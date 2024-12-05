import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { AxiosInstance } from 'axios';

export interface GetSubscriptionPortalResponse {
  url: string
}

const getSubscriptionPortal = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetSubscriptionPortalResponse>> => {
  const result: ApiResponse<GetSubscriptionPortalResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetSubscriptionPortalResponse>(`/users/${userId}/subscription-portal`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getSubscriptionPortal;
