import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetMusicCollectionSongsResponse {
  songs: MusicCollectionSongResponse[];
}

export interface MusicCollectionSongResponse {
  songPublicId: string;
  title: string;
  sequence: number;
  durationInSeconds: number;
}

const getMusicCollectionSongs = async (collectionPublicId: string): Promise<ApiResponse<GetMusicCollectionSongsResponse>> => {
  const result : ApiResponse<GetMusicCollectionSongsResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetMusicCollectionSongsResponse>(`/music-collections/${collectionPublicId}/songs`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMusicCollectionSongs;

