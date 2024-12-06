import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetMyAlbumsResponse {
  publicId: string;
  title: string,
  status: string;
  releaseDate: string
}

const getMyAlbums = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetMyAlbumsResponse[]>> => {
  const result: ApiResponse<GetMyAlbumsResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetMyAlbumsResponse[]>(`/users/${userId}/albums`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMyAlbums;

