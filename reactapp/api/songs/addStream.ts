import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addStream = async (axiosPrivate : AxiosInstance, userId: string, songPublicId: string, listeningTimeInSeconds: number): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.post<void>(`/songs/${songPublicId}/stream?userId=${userId}`,
      JSON.stringify({ listeningTimeInSeconds: listeningTimeInSeconds })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addStream;
