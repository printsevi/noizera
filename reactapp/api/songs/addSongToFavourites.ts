import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addSongToFavourites = async (axiosPrivate: AxiosInstance, userId: string, songPublicId: string): Promise<ApiResponse<IdResponse>> => {
  const result: ApiResponse<IdResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<IdResponse>(`/favourite-songs?userId=${userId}`,
      JSON.stringify({ songPublicId: songPublicId })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addSongToFavourites;
