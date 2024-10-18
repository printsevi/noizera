import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const forgotPassword = async (emailOrUsername: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPublic.patch<void>(`/auth/password?emailOrUsername=${emailOrUsername}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default forgotPassword;
