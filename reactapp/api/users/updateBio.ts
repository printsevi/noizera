import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const updateBio = async (axiosPrivate: AxiosInstance, userId: string, bio: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/users/${userId}/bio`,
      JSON.stringify({ newBio: bio })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateBio;
