import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addAlbumCredit = async (axiosPrivate : AxiosInstance, userId: string, albumId: string, creditProfileId: string): Promise<ApiResponse<IdResponse>> => {
  const result : ApiResponse<IdResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<IdResponse>(`/music-collections/albums/${albumId}/credits`,
      JSON.stringify({ creditProfileId: creditProfileId, userId: userId })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addAlbumCredit;
