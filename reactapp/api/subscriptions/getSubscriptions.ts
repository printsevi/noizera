import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';

interface GetSubscriptionsItemResponse {
  subscriptionId: string,
  subscriptionType: string,
  title: string,
  priceInEuro : number,
  profileTypes: ProfileType[],
  freeTrialInDays?: number,
  isAnnual: boolean
}

export interface GetSubscriptionsResponse {
  subscriptions: GetSubscriptionsItemResponse[]
}

const getSubscriptions = async (profileType?: ProfileType): Promise<ApiResponse<GetSubscriptionsResponse>> => {
  const result : ApiResponse<GetSubscriptionsResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetSubscriptionsResponse>(`/subscriptions?profileType=${profileType?.toString() ?? ""}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getSubscriptions;
