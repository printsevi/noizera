import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { AxiosInstance } from 'axios';

const updateMusicCollectionSongSequence = async (axiosPrivate : AxiosInstance, userId: string, musicCollectionId: string, activeSongId: string, overSongId: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put(`/music-collections/${musicCollectionId}/songs/sequences`,
      JSON.stringify({ activeSongId: activeSongId, overSongId: overSongId, userId: userId })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateMusicCollectionSongSequence;
