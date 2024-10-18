import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { AxiosInstance } from 'axios';

const deleteAlbumCredit = async (axiosPrivate : AxiosInstance, creditId: string, userId: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.delete(`/music-collections/albums/credits/${creditId}?userId=${userId}`);
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default deleteAlbumCredit;
