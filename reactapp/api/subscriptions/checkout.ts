import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { AxiosInstance } from 'axios';

export interface CheckoutResponse {
  checkoutSessionUrl: string
}

const checkout = async (axiosPrivate : AxiosInstance, userId: string, subscriptionId: string): Promise<ApiResponse<CheckoutResponse>> => {
  const result : ApiResponse<CheckoutResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<CheckoutResponse>(`/subscriptions/${subscriptionId}/checkout?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default checkout;
