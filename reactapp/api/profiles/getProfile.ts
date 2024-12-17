// 'use server'

import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetProfileResponse {
  publicId: string,
  profileType: ProfileType,
  name: string,
  isFollowing: boolean,
  bio: string,
  followersCount: number,
  followingsCount: number
}

const getProfile = async (username: string): Promise<ApiResponse<GetProfileResponse>> => {
  const result: ApiResponse<GetProfileResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetProfileResponse>(`/profiles/${username}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getProfile;

