import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { GetMusicCollectionsResponse } from './getFeedPublicCollections';
import { AxiosInstance } from 'axios';

const getFeedCollections = async (api: string, userId: string, axiosPrivate: AxiosInstance)
  : Promise<ApiResponse<GetMusicCollectionsResponse>> => {
  const result : ApiResponse<GetMusicCollectionsResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetMusicCollectionsResponse>(`/feed/collections/${api}?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedCollections;

