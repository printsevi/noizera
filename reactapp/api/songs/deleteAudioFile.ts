import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const deleteAudioFile = async (axiosPrivate: AxiosInstance, userId: string, songId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.delete<void>(`/songs/${songId}/audio?userId=${userId}`);
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default deleteAudioFile;
