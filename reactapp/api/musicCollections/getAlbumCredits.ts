import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetAlbumCreditsResponse {
  username?: string;
  name?: string;
  profileType?: ProfileType;
  profileName: string;
}

const getAlbumCredits = async (albumId: string): Promise<ApiResponse<GetAlbumCreditsResponse[]>> => {
  const result: ApiResponse<GetAlbumCreditsResponse[]> = { ok: true };
  try {
    const response = await axiosPublic.get<GetAlbumCreditsResponse[]>(`/music-collections/albums/${albumId}/credits`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getAlbumCredits;

