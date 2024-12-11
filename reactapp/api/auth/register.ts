import { axiosPublic } from '@/libs/axios';
import axios from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

const register = async (email: string, userName: string, password: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    await axiosPublic.post('/auth/register',
      JSON.stringify({ email: email, profileUserName: userName, password: password })
    );
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default register;
