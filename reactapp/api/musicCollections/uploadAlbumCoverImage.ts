import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const uploadAlbumCoverImage = async (axiosPrivate: AxiosInstance, userId: string, albumId: string, file: File, filename: string, antiforgeryToken: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    var formData = new FormData();
    formData.append("file", file, filename);
    await axiosPrivate.put<IdResponse>(`/music-collections/albums/${albumId}/cover-image?userId=${userId}`,
      formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'X-XSRF-TOKEN': antiforgeryToken,
      }
    }
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default uploadAlbumCoverImage;
