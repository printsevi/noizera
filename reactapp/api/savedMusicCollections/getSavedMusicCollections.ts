import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { AxiosInstance } from 'axios';
import { MusicCollectionResponse } from '../feed/getFeedPublicCollections';

const getSavedMusicCollections = async (userId: string, axiosPrivate: AxiosInstance)
  : Promise<ApiResponse<MusicCollectionResponse[]>> => {
  const result: ApiResponse<MusicCollectionResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<MusicCollectionResponse[]>(`/saved-collections?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getSavedMusicCollections;

