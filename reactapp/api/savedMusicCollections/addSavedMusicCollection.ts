import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const addSavedMusicCollection = async (axiosPrivate: AxiosInstance, userId: string, musicCollectionPublicId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.post(`/saved-collections?userId=${userId}`,
      JSON.stringify({ musicCollectionPublicId: musicCollectionPublicId })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default addSavedMusicCollection;
