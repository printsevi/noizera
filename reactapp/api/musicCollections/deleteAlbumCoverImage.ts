import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const deleteAlbumCoverImage = async (axiosPrivate: AxiosInstance, userId: string, albumId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.delete<void>(`/music-collections/albums/${albumId}/cover-image?userId=${userId}`);
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default deleteAlbumCoverImage;
