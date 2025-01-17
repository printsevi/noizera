// 'use server'

import { ApiResponse, CollectionType, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';
import { GetAlbumCreditsResponse } from '../musicCollections/getAlbumCredits';

export interface FeedProfileResponse {
  publicId: string,
  username: string,
  name: string
}

const getFeedPublicProfiles = async (api: string)
  : Promise<ApiResponse<FeedProfileResponse[]>> => {
  const result: ApiResponse<FeedProfileResponse[]> = { ok: true };
  try {
    const response = await axiosPublic.get<FeedProfileResponse[]>(`public/feed/profiles/${api}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedPublicProfiles;

