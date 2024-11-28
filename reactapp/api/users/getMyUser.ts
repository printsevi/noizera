import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetMyUserResponse {
  profileType: ProfileType;
  profilePublicId: string,
  username: string;
  name: string;
  activeSubscriptions: string[];
  songCount: number;
  dayLimitExceeded: boolean,
  weekLimitExceeded: boolean,
  monthLimitExceeded: boolean,
  semiAnnualLimitExceeded: boolean
}

const getMyUser = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetMyUserResponse>> => {
  const result: ApiResponse<GetMyUserResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetMyUserResponse>(`/users/${userId}/me`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMyUser;

