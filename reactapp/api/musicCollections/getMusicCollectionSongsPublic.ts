import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface MusicCollectionSongResponse {
  songPublicId: string;
  title: string;
  contentLength: number;
  sequence: number;
  durationInSeconds: number;
  albumPublicId: string;
  ownerUsername: string;
  ownerName: string;
  favouriteSongId?: string;
}

const getMusicCollectionSongsPublic = async (collectionPublicId: string): Promise<ApiResponse<MusicCollectionSongResponse[]>> => {
  const result: ApiResponse<MusicCollectionSongResponse[]> = { ok: true };
  try {
    const response = await axiosPublic.get<MusicCollectionSongResponse[]>(`/public/music-collections/${collectionPublicId}/songs`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMusicCollectionSongsPublic;

