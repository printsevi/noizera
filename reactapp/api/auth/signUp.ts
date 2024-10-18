import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const signUp = async (email: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    await axiosPublic.post('/auth/sign-up',
      JSON.stringify({ email })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default signUp;
