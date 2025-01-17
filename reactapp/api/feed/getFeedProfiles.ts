import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { FeedProfileResponse } from './getFeedPublicProfiles';
import { MusicCollectionResponse } from './getFeedPublicCollections';
import { AxiosInstance } from 'axios';

const getFeedProfiles = async (api: string, userId: string, axiosPrivate: AxiosInstance)
  : Promise<ApiResponse<FeedProfileResponse[]>> => {
  const result: ApiResponse<FeedProfileResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<FeedProfileResponse[]>(`/feed/profiles/${api}?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedProfiles;

