// 'use server'

import { ApiResponse, CollectionType, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface MusicCollectionResponse {
  publicId: string,
  title: string,
  collectionType: CollectionType,
  isSaved: boolean,
  ownerName: string,
  ownerPublicId: string,
  songCount: number
}

const getFeedPublicCollections = async (api: string)
  : Promise<ApiResponse<MusicCollectionResponse[]>> => {
  const result: ApiResponse<MusicCollectionResponse[]> = { ok: true };
  try {
    const response = await axiosPublic.get<MusicCollectionResponse[]>(`public/feed/collections/${api}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedPublicCollections;

