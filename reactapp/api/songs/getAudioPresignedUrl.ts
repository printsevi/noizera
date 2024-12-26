import { ApiResponse, handleErrorAndReturnProblem, IdResponse, UrlResponse } from '../common';
import { AxiosInstance } from 'axios';

const getAudioPresignedUrl = async (axiosPrivate: AxiosInstance, userId: string, songPublicId: string, audioType: string): Promise<ApiResponse<UrlResponse>> => {
  const result: ApiResponse<UrlResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<UrlResponse>(`/songs/${songPublicId}/audio-url?userId=${userId}&audioType=${audioType}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getAudioPresignedUrl;
