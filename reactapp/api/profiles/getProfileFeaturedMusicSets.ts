import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { MusicCollectionResponse } from '../feed/getFeedPublicCollections';

const getProfileFeaturedMusicSets = async (axiosPrivate: AxiosInstance, username: string, userId: string): Promise<ApiResponse<MusicCollectionResponse[]>> => {
  const result: ApiResponse<MusicCollectionResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<MusicCollectionResponse[]>(`/profiles/${username}/featured-music-sets?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getProfileFeaturedMusicSets;

