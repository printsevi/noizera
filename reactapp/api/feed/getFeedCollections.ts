import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { MusicCollectionResponse } from './getFeedPublicCollections';
import { AxiosInstance } from 'axios';

const getFeedCollections = async (api: string, userId: string, axiosPrivate: AxiosInstance)
  : Promise<ApiResponse<MusicCollectionResponse[]>> => {
  const result: ApiResponse<MusicCollectionResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<MusicCollectionResponse[]>(`/feed/collections/${api}?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedCollections;

