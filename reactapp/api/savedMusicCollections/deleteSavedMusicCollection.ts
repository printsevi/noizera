import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const deleteSavedMusicCollection = async (axiosPrivate: AxiosInstance, userId: string, musicCollectionPublicId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.delete(`/saved-collections/${musicCollectionPublicId}?userId=${userId}`);
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default deleteSavedMusicCollection;
