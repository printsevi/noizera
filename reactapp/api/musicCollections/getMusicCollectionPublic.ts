import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface MusicCollectionResponse {
  title: string;
  collectionType: string;
  releaseDate?: string;
  description?: string;
}

const getMusicCollectionPublic = async (collectionPublicId: string): Promise<ApiResponse<MusicCollectionResponse>> => {
  const result: ApiResponse<MusicCollectionResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<MusicCollectionResponse>(`/public/music-collections/${collectionPublicId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMusicCollectionPublic;

