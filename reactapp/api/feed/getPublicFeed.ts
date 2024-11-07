// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetPublicFeedResponse {
  musicCategories: GetPublicFeedMusicCollectionItem[];
  profileCategories: GetPublicFeedProfileItem[];
}

export interface GetPublicFeedMusicCollectionItem {
  title: string;
  items: MusicCollectionQueryResult[];
}

export interface GetPublicFeedProfileItem {
  title: string;
  items: ProfileQueryResult[];
}

export interface MusicCollectionQueryResult {
  publicId: string,
  title: string,
  collectionType: string,
}

export interface ProfileQueryResult {
  publicId: string,
  type: ProfileType,
  name: string
}

const getPublicFeed = async (): Promise<ApiResponse<GetPublicFeedResponse>> => {
  const result: ApiResponse<GetPublicFeedResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetPublicFeedResponse>(`/public-feed`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getPublicFeed;

