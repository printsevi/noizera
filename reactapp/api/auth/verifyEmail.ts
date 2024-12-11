import { axiosPublic } from '@/libs/axios';
import axios from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const verifyEmail = async (email: string, code: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPublic.post('/auth/verify-email',
      JSON.stringify({ email, code })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default verifyEmail;
