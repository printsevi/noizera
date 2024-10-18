import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const updateAlbumSongTitle = async (axiosPrivate : AxiosInstance, userId: string, albumId: string, songId: string, newTitle: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put(`/music-collections/albums/${albumId}/songs/${songId}/title`,
      JSON.stringify({ newTitle: newTitle, userId: userId })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateAlbumSongTitle;
