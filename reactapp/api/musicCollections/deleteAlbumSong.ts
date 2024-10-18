import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const deleteAlbumSong = async (axiosPrivate : AxiosInstance, userId: string, albumId: string, songId: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.delete<void>(`/music-collections/albums/${albumId}/songs/${songId}?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default deleteAlbumSong;
