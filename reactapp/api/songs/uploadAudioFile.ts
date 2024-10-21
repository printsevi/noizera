import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

export interface UploadAudioFileResponse {
  originalFileName: string;
  contentLength: number;
  contentType: string;
}

const uploadAudioFile = async (axiosPrivate: AxiosInstance, userId: string, songId: string, file: File, antiforgeryToken: string): Promise<ApiResponse<UploadAudioFileResponse>> => {
  const result: ApiResponse<UploadAudioFileResponse> = { ok: true };
  try {
    var formData = new FormData();
    formData.append("file", file);
    const response = await axiosPrivate.post<UploadAudioFileResponse>(`/songs/${songId}/audio?userId=${userId}`,
      formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'X-XSRF-TOKEN': antiforgeryToken,
      }
    }
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default uploadAudioFile;
