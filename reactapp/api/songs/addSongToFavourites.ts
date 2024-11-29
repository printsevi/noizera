import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addSongToFavourites = async (axiosPrivate: AxiosInstance, userId: string, songPublicId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/songs/${songPublicId}/favourites?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addSongToFavourites;
