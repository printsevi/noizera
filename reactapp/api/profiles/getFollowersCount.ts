import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetFollowersCountResponse {
  count: number
}

const getFollowersCount = async (profilePublicId: string): Promise<ApiResponse<GetFollowersCountResponse>> => {
  const result : ApiResponse<GetFollowersCountResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetFollowersCountResponse>(`/profiles/${profilePublicId}/followers-count`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFollowersCount;

