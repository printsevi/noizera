// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

export interface GetArtistResponse {
  artistId?: string;
  name: string;
  publicId: string,
  username: string;
}

const getArtists = async (axiosPrivate: AxiosInstance, text: string, userId: string): Promise<ApiResponse<GetArtistResponse[]>> => {
  const result: ApiResponse<GetArtistResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetArtistResponse[]>(`/profiles/artists?text=${text}&userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getArtists;

