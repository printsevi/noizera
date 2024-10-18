import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { AxiosInstance } from 'axios';

const updateMusicCollectionTitle = async (axiosPrivate : AxiosInstance, userId: string, musicCollectionId: string, newTitle: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.put(`/music-collections/${musicCollectionId}/title`,
      JSON.stringify({ newTitle: newTitle, userId: userId })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateMusicCollectionTitle;
