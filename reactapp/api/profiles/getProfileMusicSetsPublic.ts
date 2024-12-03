import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { MusicCollectionResponse } from '../feed/getFeedPublicCollections';
import { axiosPublic } from '@/libs/axios';

const getProfileMusicSetsPublic = async (username: string): Promise<ApiResponse<MusicCollectionResponse[]>> => {
  const result: ApiResponse<MusicCollectionResponse[]> = { ok: true };
  try {
    const response = await axiosPublic.get<MusicCollectionResponse[]>(`/public/profiles/${username}/music-sets`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getProfileMusicSetsPublic;

