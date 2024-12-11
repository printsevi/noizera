import axios, { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const signOut = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPrivate.post('/auth/sign-out',
      JSON.stringify({ userId: userId })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default signOut;
