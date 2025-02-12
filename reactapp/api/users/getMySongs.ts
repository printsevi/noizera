import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetMySongsResponse {
  publicId: string;
  title: string,
  streamCount: number;
  likeCount: string
}

const getMySongs = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetMySongsResponse[]>> => {
  const result: ApiResponse<GetMySongsResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetMySongsResponse[]>(`/users/${userId}/songs`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMySongs;

