import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

export interface AddAlbumSongResponse {
  songId: string;
  songPublicId: string;
}

const addAlbumSong = async (axiosPrivate : AxiosInstance, userId: string, albumId: string): Promise<ApiResponse<AddAlbumSongResponse>> => {
  const result : ApiResponse<AddAlbumSongResponse> = { ok: true };
  try {
    const response = await axiosPrivate.post<AddAlbumSongResponse>(`/music-collections/albums/${albumId}/songs`,
      JSON.stringify({ userId: userId })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addAlbumSong;
