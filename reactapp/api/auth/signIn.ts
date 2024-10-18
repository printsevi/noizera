import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const signIn = async (emailOrUsername: string, password: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    await axiosPublic.post('/auth/sign-in',
      JSON.stringify({ emailOrUsername: emailOrUsername, password: password })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default signIn;
