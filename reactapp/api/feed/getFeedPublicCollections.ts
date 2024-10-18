// 'use server'

import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetMusicCollectionsResponse {
  items: MusicCollectionResponse[];
}

export interface MusicCollectionResponse {
  publicId: string,
  title: string,
  collectionType: string
}

const getFeedPublicCollections = async (api: string)
  : Promise<ApiResponse<GetMusicCollectionsResponse>> => {
  const result : ApiResponse<GetMusicCollectionsResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetMusicCollectionsResponse>(`/feed/collections/${api}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedPublicCollections;

