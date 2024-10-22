import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const updateName = async (axiosPrivate: AxiosInstance, userId: string, name: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/users/${userId}/name`,
      JSON.stringify({ newName: name })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateName;
