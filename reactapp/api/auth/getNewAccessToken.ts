import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

export interface GetNewAccessTokenResponse {
  accessToken: string;
  refreshToken: string;
}

const getNewAccessToken = async (refreshToken: string, expiredAccessToken: string): Promise<ApiResponse<GetNewAccessTokenResponse>> => {
  const result : ApiResponse<GetNewAccessTokenResponse> = { ok: true };
  try {
    const response = await axiosPublic.put<GetNewAccessTokenResponse>(`/auth/access-token`,
      JSON.stringify({ refreshToken: refreshToken, expiredAccessToken: expiredAccessToken })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getNewAccessToken;
