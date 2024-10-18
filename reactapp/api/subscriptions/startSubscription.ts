import { ApiResponse, handleErrorAndReturnProblem, ProfileType, SubscriptionType } from '../common';
import { AxiosInstance } from 'axios';

export interface StartSubscriptionResponse {
  activeSubscriptionType: SubscriptionType
}

const startSubscription = async (axiosPrivate : AxiosInstance, userId: string, checkoutId: string): Promise<ApiResponse<StartSubscriptionResponse>> => {
  const result : ApiResponse<StartSubscriptionResponse> = { ok: true };
  try {
    const response = await axiosPrivate.put<StartSubscriptionResponse>(`/subscriptions/checkouts/${checkoutId}/success?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default startSubscription;
