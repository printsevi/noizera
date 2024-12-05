import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';
import { GetSubscriptionPortalResponse } from './getSubscriptionPortal';

const getOrCreateConnectedAccount = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetSubscriptionPortalResponse>> => {
  const result: ApiResponse<GetSubscriptionPortalResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<GetSubscriptionPortalResponse>(`/users/${userId}/connected-account`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getOrCreateConnectedAccount;
