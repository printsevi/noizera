import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addNewAlbumCredit = async (axiosPrivate: AxiosInstance, userId: string, albumId: string, creditProfileName: string): Promise<ApiResponse<IdResponse>> => {
  const result: ApiResponse<IdResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<IdResponse>(`/music-collections/albums/${albumId}/new-credits?userId=${userId}`,
      JSON.stringify({ creditProfileName: creditProfileName })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addNewAlbumCredit;
