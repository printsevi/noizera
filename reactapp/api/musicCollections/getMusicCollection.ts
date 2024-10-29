import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';
import { MusicCollectionResponse } from './getMusicCollectionPublic';

const getMusicCollection = async (collectionPublicId: string, axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<MusicCollectionResponse>> => {
  const result: ApiResponse<MusicCollectionResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<MusicCollectionResponse>(`/music-collections/${collectionPublicId}?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMusicCollection;

