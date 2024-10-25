import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';
import { AxiosInstance } from 'axios';

const updateAlbumReleaseDate = async (axiosPrivate: AxiosInstance, userId: string, albumId: string, releaseDate: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.put(`/music-collections/albums/${albumId}/release-date?userId=${userId}`,
      JSON.stringify({ newDate: releaseDate })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateAlbumReleaseDate;
