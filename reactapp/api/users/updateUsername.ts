import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const updateUsername = async (axiosPrivate : AxiosInstance, userId: string, username: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/users/${userId}/username`,
      JSON.stringify({ newUsername: username })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateUsername;
