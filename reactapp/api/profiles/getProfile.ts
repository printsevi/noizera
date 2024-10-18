// 'use server'

import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetProfileResponse {
  profileImageSrc: string,
  name?: string,
  isFollowing: boolean,
  bio: string,
  followersCount: number,
  followingsCount: number
}

const getProfile = async (profilePublicId: string): Promise<ApiResponse<GetProfileResponse>> => {
  const result : ApiResponse<GetProfileResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetProfileResponse>(`/profiles/${profilePublicId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getProfile;

