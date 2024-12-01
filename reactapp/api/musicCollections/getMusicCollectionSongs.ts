import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';
import { MusicCollectionSongResponse } from './getMusicCollectionSongsPublic';


const getMusicCollectionSongs = async (collectionPublicId: string, audioType: string, axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<MusicCollectionSongResponse[]>> => {
  const result: ApiResponse<MusicCollectionSongResponse[]> = { ok: true };
  try {
    const response = await axiosPrivate.get<MusicCollectionSongResponse[]>(`/music-collections/${collectionPublicId}/songs?audioType=${audioType}&userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getMusicCollectionSongs;

